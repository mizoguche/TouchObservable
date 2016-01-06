using UnityEngine;

public struct SingleTouch
{
    public Vector3 Position;
    public Vector3 DeltaPosition;

    public SingleTouch(Vector3 position, Vector3 deltaPosition)
    {
        Position = position;
        DeltaPosition = deltaPosition;
    }
}
