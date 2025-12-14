using System;

[Flags]
public enum HitboxLayer
{
  PlayerHitbox = PhysicsLayers.PlayerHitbox,
  EnemyHitbox = PhysicsLayers.EnemyHitbox,
}