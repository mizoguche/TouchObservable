using UnityEngine;

public struct SingleTouch
{
    public Vector2 Position;
    public Vector2 DeltaPosition;

    public SingleTouch(Vector2 position, Vector2 deltaPosition)
    {
        Position = position;
        DeltaPosition = deltaPosition;
    }
}
