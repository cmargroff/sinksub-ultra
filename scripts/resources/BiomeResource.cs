using Godot;
using Godot.Collections;

namespace SinkSub.Resources;

[GlobalClass]
public partial class BiomeResource : Resource
{
  [Export]
  public string BiomeName { get; set; } = "New Biome";
  [Export]
  public string BiomeKey { get; set; } = "new_biome";
  [Export]
  public Texture2D TransitionTexture { get; set; }
}