using UnityEngine;

namespace DebugUtils
{
    public static class DebugDraw
    {
        public static void X(Vector2Int target)
        {
            Debug.DrawLine(new Vector3(target.x - 0.5f, target.y - 0.5f, 1f), new Vector3(target.x + .5f, target.y + .5f, 1f), Color.red, 2.0f);
            Debug.DrawLine(new Vector3(target.x - 0.5f, target.y + 0.5f, 1f), new Vector3(target.x + .5f, target.y - .5f, 1f), Color.red, 2.0f);
        }
    }
}