using System.Linq;
using Godot;

namespace SinkSub.Util;

public static class Node3DExtensions
{
  public static T[] GetChildrenOfType<T>(this Node3D node, bool includeInternal = false, bool recursive = false) where T : Node
  {
    if (recursive)
    {
      return node.GetChildren(includeInternal).Where(c => c is T).Cast<T>()
        .Concat(node.GetChildren(includeInternal).OfType<Node3D>()
        .SelectMany(n => n.GetChildrenOfType<T>(includeInternal, true))).ToArray();
    }
    return node.GetChildren(includeInternal).Where(c => c is T).Cast<T>().ToArray();
  }
}