using System;

[Flags]
public enum HurtboxLayer
{
  PlayerHurtbox = PhysicsLayers.PlayerHurtbox,
  EnemyHurtbox = PhysicsLayers.EnemyHurtbox,
}