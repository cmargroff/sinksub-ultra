using Godot;

namespace SinkSub.Actors;

public partial class Ship : Node3D
{
  [Export] public float Acceleration = 2f;
  [Export] public float MaxSpeed = 20f;
  [Export(PropertyHint.Range, "0,1,0.01")] public float FrictionCoefficient = 0.98f;
  private float _speed = 0f;
  private Mover _mover;
  private Camera3D _camera;
  private Marker3D _leftSensor;
  private Marker3D _rightSensor;

  public override void _Ready()
  {
    _camera = GetTree().Root.GetCamera3D();
    _mover = GetNode<Mover>("%Mover");
    if (_mover == null)
    {
      GD.PrintErr("Ship: Mover node not found!");
    }
    _leftSensor = GetNode<Marker3D>("%SensorLeft");
    _rightSensor = GetNode<Marker3D>("%SensorRight");
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
    CheckSensors();
  }

  private void CheckSensors()
  {
    if (!_camera.IsPositionInFrustum(_leftSensor.GlobalPosition))
    {
      if (_speed < 0)
      {
        _speed = 0;
        _mover.SpeedX = 0;
      }
    }
    else if (!_camera.IsPositionInFrustum(_rightSensor.GlobalPosition))
    {
      if (_speed > 0)
      {
        _speed = 0;
        _mover.SpeedX = 0;
      }
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