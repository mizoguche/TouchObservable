using System;
using UnityEngine;

public class Cube : MonoBehaviour, Touchable
{
    public void OnTouch()
    {
        Debug.Log("Cube touched.");
    }
}
