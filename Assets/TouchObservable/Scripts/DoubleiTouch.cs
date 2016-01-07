using System.Collections.Generic;
using UnityEngine;

public struct DoubleTouch
{
    public List<SingleTouch> Touches;
    public float Distance;

    public DoubleTouch(List<SingleTouch> touches, float distance)
    {
        this.Touches = touches;
        this.Distance = distance;
    }
}
