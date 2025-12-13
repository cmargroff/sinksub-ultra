using Godot;
using SinkSub.Managers.Interfaces;
using SinkSub.Util;

namespace SinkSub.Managers;

public partial class GameManager : Node
{
  private ISceneManager _sceneManager;
  [FromServices]
  public void Inject(ISceneManager sceneManager)
  {
    GD.Print(GetType().Name, " Constructed");
    _sceneManager = sceneManager;
  }
  public override void _Ready()
  {
    GD.Print(GetType().Name, " Started");
    var InitialSceneName = ProjectSettings.GetSetting("custom/initial_scene").AsString();
    var currentScene = GetTree().CurrentScene;
    if (currentScene.Name != "Entry")
    {
      // game was started by running a scene directly with F6
      // InitialSceneName = currentScene.Name;
      // currentScene.QueueFree();
    }
    else if (InitialSceneName != "")
    {
      _sceneManager.ChangeScene(InitialSceneName);
    }
  }
}
