using Godot;

namespace SinkSub.Util;

public static class Physics
{
  public static int CalculateHitstopDuration(float damageAmount, float multiplier = 1f, bool wasBlocked = false)
  {
    float shieldMultiplier = wasBlocked ? 0.8f : 1f;
    var frames = (damageAmount * 0.15f + 1f) * multiplier * shieldMultiplier;
    return (int)Mathf.Clamp(frames, 0, 30);
  }
  public static float CalculateKnockbackForce(float hitDamage, float weight, float kbg)
  {
    var kb = (((0.1f + hitDamage / 20f) * (200 / (weight + 100f)) * 1.4f) + 18f) * kbg;
    return kb;
  }
}