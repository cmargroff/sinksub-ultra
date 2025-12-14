using Godot;

[GlobalClass]
public partial class Mover : Node
{
  private float _speedX;
  [Export]
  public float SpeedX
  {
    get => _speedX; set
    {
      _speedX = value;
      UpdateVelocity();
    }
  }
  private float _speedY;
  [Export]
  public float SpeedY
  {
    get => _speedY; set
    {
      _speedY = value;
      UpdateVelocity();
    }
  }
  private Node3D _target;
  private Vector3 _velocity;
  public override void _EnterTree()
  {
    _target = GetParent<Node3D>();
  }

  private void UpdateVelocity()
  {
    _velocity = new Vector3(SpeedX, SpeedY, 0) * (1f / Engine.PhysicsTicksPerSecond);
  }

  public override void _PhysicsProcess(double delta)
  {
    _target.GlobalPosition += _velocity;
  }

  public void FlipX()
  {
    SpeedX = -SpeedX;
  }
  public void FlipY()
  {
    SpeedY = -SpeedY;
  }
}
