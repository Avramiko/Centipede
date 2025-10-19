using UnityEngine;

public static class GridUtility
{
    public static Vector2 SnapToGrid(Vector2 position, float spacingX, float spacingY)
    {
        float snappedX = Mathf.Round(position.x / spacingX) * spacingX;
        float snappedY = Mathf.Round(position.y / spacingY) * spacingY;

        return new Vector2(snappedX, snappedY);
    }

    public static Vector2Int WorldToGridKey(Vector2 position, float spacingX, float spacingY)
    {
        int x = Mathf.RoundToInt(position.x / spacingX);
        int y = Mathf.RoundToInt(position.y / spacingY);

        return new Vector2Int(x, y);
    }

    public static Vector2 ClampToBounds(Vector2 position, Rect bounds)
    {
        float x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
        float y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax);

        return new Vector2(x, y);
    }
}