using Godot;
using SinkSub.Models;

namespace SinkSub.Components;

[GlobalClass]
public partial class Hitbox : Area3D
{
  [Signal]
  public delegate void HitStopEventHandler(int duration);
  public HitboxProperties Properties;
  private Area3D _area;
  private CollisionShape3D _collider;
  public override void _EnterTree()
  {
    if (Properties == null)
    {
      QueueFree();
      return;
    }
    _area = new Area3D();
    // _area.Monitoring = false;
    _area.InputRayPickable = false;
    _collider = new CollisionShape3D();
    UpdateProperties(Properties);
    _area.AddChild(_collider);
    AddChild(_area);
  }

  public void UpdateProperties(HitboxProperties props)
  {
    Properties = props;
    _area.Position = Properties.Offset;
    CalculateShape(_collider);

    _area.CollisionLayer = (uint)Properties.CollisionLayer;
    _area.CollisionMask = (uint)Properties.CollisionMask;
  }

  public void CalculateShape(CollisionShape3D collider)
  {
    if (Properties.IsCapsule == false)
    {
      collider.Shape = new SphereShape3D() { Radius = Properties.Radius };
    }
    else
    {
      collider.Shape = new CapsuleShape3D()
      {
        Radius = Properties.Radius,
        Height = Properties.Height
      };
      collider.RotationDegrees = Properties.Rotation;
    }
  }


#if DEBUG
  public override void _Ready()
  {
    var material = new StandardMaterial3D()
    {
      AlbedoColor = new Color(1, 0, 0, 0.4f),
      ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded,
      Transparency = BaseMaterial3D.TransparencyEnum.AlphaDepthPrePass,
      StencilColor = new Color(1, 0, 0, 0.4f),
      StencilMode = BaseMaterial3D.StencilModeEnum.Xray
    };
    Mesh mesh = null;
    if (Properties.IsCapsule)
    {
      mesh = new CapsuleMesh()
      {
        Radius = Properties.Radius,
        Height = Properties.Height,
        RadialSegments = 12,
        Rings = 8,
        Material = material,
      };
    }
    else
    {
      mesh = new SphereMesh()
      {
        Radius = Properties.Radius,
        Height = Properties.Radius * 2,
        RadialSegments = 12,
        Rings = 8,
        Material = material,
      };
    }
    var meshInstance = new MeshInstance3D();
    meshInstance.RotationDegrees = Properties.Rotation;
    meshInstance.Mesh = mesh;
    meshInstance.Layers = (uint)CameraLayers.Hitboxes;
    AddChild(meshInstance);
    meshInstance.Position = Properties.Offset;
  }
#endif
}