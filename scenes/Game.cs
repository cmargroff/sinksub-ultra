using Godot;
using SinkSub.Managers.Interfaces;
using SinkSub.Resources;
using SinkSub.Util;

namespace SinkSub.Scenes;

public partial class Game : Node3D
{
  private IRunManager _runManager;
  [FromServices]
  public void Inject(IRunManager runManager)
  {
    _runManager = runManager;
  }
  public override void _Ready()
  {
    // game started
    // read test biome
    var biome = ResourceLoader.Load<BiomeResource>("res://resources/biomes/test.tres");
    // tell run manager we have a new biome
    _runManager.ChangeBiome(biome);
  }
}
