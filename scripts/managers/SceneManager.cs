using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using Microsoft.Extensions.DependencyInjection;
using SinkSub.Managers.Interfaces;

namespace SinkSub.Managers;

using Preloaded = Dictionary<string, Dictionary<string, Resource>>;
using Preloads = Dictionary<string, Dictionary<string, string>>;

public partial class SceneManager : Node3D, ISceneManager
{
  [Signal]
  public delegate void LoadingShownEventHandler();
  [Signal]
  public delegate void LoadingHiddenEventHandler();
  private Control _loadingScene;
  private Node _currentScene;
  public Preloaded PreloadedResources;
  private Preload _currentLoading;
  private Godot.Collections.Array _currentProgress;
  private List<Preload> _preloadQueue;
  private string _nextName;
  private bool _processing = false;
  private bool _ready = false;
  public override void _Ready()
  {
    GD.Print(GetType().Name, " Ready");
  }
  public override void _EnterTree()
  {
    GD.Print(GetType().Name, " Enter");
    _loadingScene = GetNode("%LoadingScene") as Control;
  }
  public void ShowLoading()
  {
    if (!_ready) return;
    _loadingScene.Visible = true;
  }
  public void HideLoading()
  {
    _loadingScene.Visible = false;
  }
  private void FreePreloads()
  {
    if (PreloadedResources != null)
    {
      foreach (var section in PreloadedResources)
      {
        foreach (var resource in section.Value)
        {
          if (!resource.Value.IsQueuedForDeletion())
          {
            //resource.Value.Free(); //resources are automatically freed when there are 0 references to them
          }
        }
      }
    }
    PreloadedResources = new();
  }
  private void ListPreloads(Preloads preloads)
  {
    _preloadQueue = new();

    foreach (var section in preloads)
    {
      foreach (var resource in section.Value)
      {
        _preloadQueue.Add(new Preload { Group = section.Key, Key = $"{resource.Key}", Path = resource.Value });
      }
    }
  }
  private void StartPreloads()
  {
    GD.Print("Starting preloads");
    if (_preloadQueue == null) return;
    NextResource();
    _processing = true;
  }
  private void MapPreloaded()
  {
    var resource = ResourceLoader.Singleton.LoadThreadedGet(_currentLoading.Path);
    if (!PreloadedResources.ContainsKey(_currentLoading.Group))
    {
      PreloadedResources.Add(_currentLoading.Group, new());
    }
    PreloadedResources[_currentLoading.Group].Add(_currentLoading.Key, resource);
  }
  private void NextResource()
  {
    if (_preloadQueue.Count > 0)
    {
      var preload = _preloadQueue.First();
      _preloadQueue.RemoveAt(0);
      _currentLoading = preload;
      ResourceLoader.Singleton.LoadThreadedRequest(_currentLoading.Path);
      return;
    }
    // if there is nothing left to load then jump to finished preloading
    FinishedPreloading();
  }

  /// <summary>
  /// Run every frame while the game is preloading resources.
  /// Throws an exception if something wasnt loaded.
  /// Calls loading next resource if resource thread is complete.
  /// </summary>
  /// <exception cref="Exception"></exception>
  private void CheckPreload()
  {
    if (!_processing) return;
    var status = ResourceLoader.Singleton.LoadThreadedGetStatus(_currentLoading.Path, _currentProgress);
    switch (status)
    {
      case ResourceLoader.ThreadLoadStatus.Loaded:
        MapPreloaded();
        NextResource();
        return;
      case ResourceLoader.ThreadLoadStatus.InvalidResource:
      case ResourceLoader.ThreadLoadStatus.Failed:
        throw new Exception("Failed to load resource " + _currentLoading.Path);
    }
    // nothing was handled so this will fall through to check next frame
  }
  public override void _Process(double delta)
  {
    CheckPreload();
  }
  public void ChangeScene(string name)
  {
    ChangeScene(name, new());
  }
  public void ChangeScene(string name, Preloads preloads)
  {
    GD.Print("Starting scene change to ", name);
    _nextName = name;
    EmitSignal(SignalName.LoadingShown);
    ShowLoading();

    if (_currentScene != null)
      _currentScene.QueueFree();

    App.CloseSceneScope();

    FreePreloads();

    if (preloads.Count > 0)
    {
      ListPreloads(preloads);
      StartPreloads();
    }
    else
    {
      LoadNextScene();
    }
  }
  private void FinishedPreloading()
  {
    _processing = false;

    LoadNextScene();
  }
  private void LoadNextScene()
  {
    if (_nextName != null)
    {
      // load the next scene and append it to the manager
      App.CreateSceneScope();
      GD.Print("Loading scene ", _nextName);
      var scene = App.ServiceProvider.GetKeyedService<Node>(_nextName);
      scene.Connect(Node.SignalName.Ready, Callable.From(SceneFinishedLoading));
      GD.Print("Runnning scene ", _nextName);
      AddChild(scene);

      _currentScene = scene;
    }
  }
  private void SceneFinishedLoading()
  {
    EmitSignal(SignalName.LoadingHidden);
    HideLoading();
  }
  private class Preload
  {
    public string Group;
    public string Key;
    public string Path;
  }

  public static string[] ListAvailableScenes()
  {
    return ResourceLoader.ListDirectory("res://scenes")
      .Where(
        path => SceneFileNameRegex().Match(path).Success
      )
      .Select(rel => $"res://scenes/{rel}")
      .ToArray();
  }

  [GeneratedRegex("\\.t?scn$")]
  private static partial Regex SceneFileNameRegex();
}
