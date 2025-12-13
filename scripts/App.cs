using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using SinkSub.Managers;
using SinkSub.Managers.Interfaces;
using SinkSub.Util;

namespace SinkSub;

public partial class App : Node
{
  private static ServiceProvider _serviceProvider;
  private static IServiceScope _currentScope;
  public static IServiceProvider ServiceProvider
  {
    get
    {
      if (_currentScope != null) return _currentScope.ServiceProvider;
      return _serviceProvider;
    }
  }

  public override void _EnterTree()
  {
#if DEBUG
    var epoch = DateTime.Now;
    var memAmount = GC.GetTotalMemory(true);
#endif
    var services = new ServiceCollection();

#if DEBUG
    GD.Print("Memory used for service collection: " + (GC.GetTotalMemory(true) - memAmount) + " bytes");
#endif

    services
    .AddSingleton(InjectNodeClass<GameManager>(true))
    .AddSingleton<ISceneManager>(InjectInstantiatedPackedScene<SceneManager>("res://views/SceneManager.tscn"))
    .AddSingleton<IRunManager, RunManager>()
    ;

    AddScenes(services);

#if DEBUG
    GD.Print("Memory used during service registration: " + (GC.GetTotalMemory(true) - memAmount) + " bytes");
    GD.Print("Time taken during service registration: " + (DateTime.Now - epoch).TotalMilliseconds + " ms");
#endif

    _serviceProvider = services.BuildServiceProvider();
    CreateSceneScope();
    _serviceProvider.GetRequiredService<GameManager>();
  }

  private Func<IServiceProvider, T> InjectNodeClass<T>(bool autoParent = false) where T : Node, new()
  {
    return (serviceProvider) =>
    {
      var node = new T();
      node.Name = typeof(T).Name + "_DI_Managed";

      InjectAttributedMethods(node);

      if (autoParent)
      {
        AddChild(node);
      }
      return node;
    };
  }
  private Func<IServiceProvider, T> InjectInstantiatedPackedScene<T>(string path, bool autoParent = true) where T : Node
  {
    return (serviceProvider) =>
    {
      var packed = ResourceLoader.Load<PackedScene>(path);
      var node = packed.Instantiate<T>();
      node.Name = typeof(T).Name + "_DI_Managed";

      InjectAttributedMethods(node);

      if (autoParent)
      {
        GetTree().Root.CallDeferred("add_child", node);
      }
      return node;
    };
  }
  public static void InjectAttributedMethods<T>(T obj)
  {
    var objType = obj.GetType();
    var methods = objType
      .GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(method => method.GetCustomAttribute<FromServicesAttribute>() != null);

    foreach (var method in methods)
    {
      var args = method
        .GetParameters()
        .Select(param => _currentScope.ServiceProvider.GetService(param.ParameterType)).ToArray();
      method.Invoke(obj, args);
    }

    var objFields = objType.GetFields(BindingFlags.Instance | BindingFlags.Public)
    .Where(fieldInfo => !fieldInfo.FieldType.IsValueType && fieldInfo.FieldType.IsClass);

    foreach (var fieldInfo in objFields)
    {
      var val = fieldInfo.GetValue(obj);
      if (val != null)
        InjectAttributedMethods(val);
    }
    // inject children in the scene tree
    if (obj is Node node && node.GetChildCount() > 0)
    {
      foreach (var child in node.GetChildren())
      {
        InjectAttributedMethods(child);
      }
    }
  }

  public void AddScenes(IServiceCollection collection)
  {
    var paths = SceneManager.ListAvailableScenes();
    foreach (var path in paths)
    {
      collection.AddKeyedTransient(Path.GetFileNameWithoutExtension(path), InjectAvailableScene(path));
    }
  }

  public Func<IServiceProvider, object, Node> InjectAvailableScene(string path)
  {
    return (ServiceProvider, serviceKey) => InjectInstantiatedPackedScene<Node>(path, false)(ServiceProvider);
  }

  public static void CreateSceneScope()
  {
    if (_currentScope is not null)
      throw new InvalidOperationException("You must close the service scope before opening a new one. Call " + nameof(CloseSceneScope) + "().");
    _currentScope = _serviceProvider.CreateScope();
  }

  public static void CloseSceneScope()
  {
    _currentScope?.Dispose();
    _currentScope = null;
  }
}