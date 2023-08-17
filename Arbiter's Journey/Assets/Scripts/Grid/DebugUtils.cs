using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;
using static UnityEngine.GraphicsBuffer;

namespace DebugUtils
{
    public static class DebugDraw
    {
        public static void X(Vector2Int target)
        {
            X(target, Color.red);
        }

        internal static void X(Vector2Int target, Color color)
        {
            Debug.DrawLine(new Vector3(target.x - 0.5f, target.y - 0.5f, 1f), new Vector3(target.x + .5f, target.y + .5f, 1f), color, 5.0f);
            Debug.DrawLine(new Vector3(target.x - 0.5f, target.y + 0.5f, 1f), new Vector3(target.x + .5f, target.y - .5f, 1f), color, 5.0f);
        }

        internal static void DebugPath(List<PathNode> debugPath)
        {
            if (debugPath != null)
            {
                foreach (PathNode pathNode in debugPath)
                {
                    DebugDraw.X(pathNode.GetLocation(), Color.white);
                }
            }
        }
    }
}