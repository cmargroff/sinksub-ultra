using Godot;

namespace SinkSub.Util;

public static class Vector2Extensions
{
  public static Vector2I ToVector2I(this Vector2 vec)
  {
    return new Vector2I(Mathf.RoundToInt(vec.X), Mathf.RoundToInt(vec.Y));
  }
  public static Vector2 Wrap(this Vector2 vec, float min, float max)
  {
    vec.X = Mathf.Wrap(vec.X, min, max);
    vec.Y = Mathf.Wrap(vec.Y, min, max);
    return vec;
  }
}