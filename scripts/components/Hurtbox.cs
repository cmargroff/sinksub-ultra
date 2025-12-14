using Godot;

namespace SinkSub.Components;

[Tool]
[GlobalClass]
public partial class Hurtbox : Area3D
{
#if DEBUG
  private MeshInstance3D _debugMesh;
  private StandardMaterial3D _debugMaterial;
  public override void _Ready()
  {
    var collisionShape = GetChild<CollisionShape3D>(0);
    var shape = collisionShape.Shape;
    _debugMesh = new MeshInstance3D();
    _debugMaterial = new StandardMaterial3D()
    {
      AlbedoColor = new Color(1, 1, 1, 0.2f),
      ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
      Transparency = BaseMaterial3D.TransparencyEnum.AlphaDepthPrePass,
    };
    if (shape is SphereShape3D sphere)
    {
      _debugMesh.Mesh = new SphereMesh()
      {
        Radius = sphere.Radius,
        Height = sphere.Radius * 2,
        RadialSegments = 12,
        Rings = 6
      };
    }
    else if (shape is CapsuleShape3D capsule)
    {
      _debugMesh.Mesh = new CapsuleMesh()
      {
        Radius = capsule.Radius,
        Height = capsule.Height,
        RadialSegments = 12,
        Rings = 6
      };
    }
    _debugMesh.MaterialOverride = _debugMaterial;
    _debugMaterial.StencilMode = BaseMaterial3D.StencilModeEnum.Xray;
    _debugMaterial.StencilColor = new Color(1, 1, 1, 0.25f);
    _debugMesh.Layers = (uint)CameraLayers.Hurtbox;
    AddChild(_debugMesh);
    _debugMesh.Position = collisionShape.Position;
    _debugMesh.Rotation = collisionShape.Rotation;
  }
#endif
}