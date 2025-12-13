using Godot;
using System;

[GlobalClass]
public partial class Mover : Node3D
{
    [Export] public float SpeedX;
    [Export] public float SpeedY;
    private Area3D _target;

    public override void _EnterTree()
    {
        _target = GetParent<Area3D>();
    }

    public override void _Process(double delta)
    {
        _target.GlobalPosition += new Vector3(SpeedX, SpeedY, 0) * (float)delta; 
    }

}
