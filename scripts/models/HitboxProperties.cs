using Godot;

namespace SinkSub.Models;

public class HitboxProperties
{
  public int ID = 0;
  /// <summary>
  /// The group ID this hitbox belongs to, if multiple hitboxes share the same group ID, only one hit will be registered per attack.
  /// </summary>
  public int Group = 0;
  public int RehitDelay = 0;
  public float Damage = 0f;
  /// <summary>
  /// The size (radius) of the hitbox.
  /// </summary>
  public float Radius = 0.5f;
  public float Height = 1f;
  public Vector3 Offset = Vector3.Zero;
  public Vector3 Rotation = Vector3.Zero;
  public bool IsCapsule = false;
  public Vector3 Stretch;
  public float HitLagMultiplier = 1f;
  public HitboxLayer CollisionLayer = HitboxLayer.PlayerHitbox;
  public HurtboxLayer CollisionMask = HurtboxLayer.EnemyHurtbox;
}