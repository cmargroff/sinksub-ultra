using Godot;

namespace SinkSub.Actors;

public partial class Ship : Node3D
{
  [Export] public float Acceleration = 2f;
  [Export] public float MaxSpeed = 20f;
  [Export(PropertyHint.Range, "0,1,0.01")] public float FrictionCoefficient = 0.98f;
  private float _speed = 0f;
  private Mover _mover;

  public override void _Ready()
  {
    _mover = GetNode<Mover>("Mover");
    if (_mover == null)
    {
      GD.PrintErr("Ship: Mover node not found!");
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    if (Input.IsActionPressed("ship_left"))
    {
      _speed -= Acceleration * (float)delta;
      UpdateSpeed();
    }
    else if (Input.IsActionPressed("ship_right"))
    {
      _speed += Acceleration * (float)delta;
      UpdateSpeed();
    }

    if (Input.IsActionJustPressed("launch_left"))
    {

    }
    if (Input.IsActionJustPressed("launch_right"))
    {

    }
  }

  private void UpdateSpeed()
  {
    _speed = Mathf.Clamp(_speed, -MaxSpeed, MaxSpeed);
    _mover.SpeedX = _speed;
    _speed *= FrictionCoefficient;
  }

  private void LaunchCharge()
  {

  }
}