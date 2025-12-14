using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SinkSub.Util;

namespace SinkSub.Components;

[Tool]
[GlobalClass]
public partial class Damageable : Node3D
{
  private Dictionary<int, int> _damageTracker = new();
  public event Action<Hitbox> Damaged;
  [Export]
  public Skeleton3D Skeleton;
  private HurtboxLayer _collisionLayer;
  [Export]
  public HurtboxLayer CollisionLayer
  {
    get => _collisionLayer;
    set
    {
      _collisionLayer = value;
      UpdateLayers();
    }
  }
  private HitboxLayer _collisionMask;
  [Export]
  public HitboxLayer CollisionMask
  {
    get => _collisionMask;
    set
    {
      _collisionMask = value;
      UpdateLayers();
    }
  }
  private List<Hurtbox> Hurtboxes = new();
  public override void _Ready()
  {
    if (Engine.IsEditorHint()) return;
    FindHurtboxes();
    ConnectHurtboxSignals();
  }
  private void FindHurtboxes()
  {
    Hurtboxes = this.GetChildrenOfType<Hurtbox>(true, true).ToList();
  }
  private void UpdateLayers()
  {
    if (Hurtboxes == null) return;
    foreach (var hurtbox in Hurtboxes)
    {
      if (!hurtbox.IsNodeReady()) continue;
    }
  }
  private void ConnectHurtboxSignals()
  {
    if (Hurtboxes == null) return;
    foreach (var hurtbox in Hurtboxes)
    {
      hurtbox.AreaEntered += Hurtbox_AreaEntered;
    }
  }
  private void Hurtbox_AreaEntered(Node3D area)
  {
    if (area.GetParent() is Hitbox hitbox)
    {
      var stop = Physics.CalculateHitstopDuration(hitbox.Properties.Damage, hitbox.Properties.HitLagMultiplier);
      // you need to extend the rehit delay by the hitstop duration so that it doesnt damage again immediately after hitstop
      if (hitbox.Properties.RehitDelay > 0)
      {
        if (_damageTracker.ContainsKey(hitbox.Properties.Group))
        {
          int lastHitFrame = _damageTracker[hitbox.Properties.Group];
          int currentFrame = Engine.GetFramesDrawn();
          if (currentFrame - lastHitFrame < hitbox.Properties.RehitDelay)
          {
            // too soon to reapply damage from this hitbox
            return;
          }
        }
        // update the last hit frame
        _damageTracker[hitbox.Properties.Group] = Engine.GetFramesDrawn() + stop;
      }
      // do filtering based on group of hitbox
      GD.Print("Hitbox entered hurtbox: " + hitbox.Name);
      hitbox.EmitSignal("HitStop", stop);
      Damaged?.Invoke(hitbox);
    }
  }
}
