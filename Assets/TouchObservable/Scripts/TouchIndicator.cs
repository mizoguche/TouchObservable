using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class TouchIndicator : MonoBehaviour
{
    [SerializeField]
    List<Image> indicators;

    public void SetPosition(DoubleTouch touches)
    {
        setPosition(indicators[0].GetComponent<RectTransform>(), touches.Touches[0]);
        setPosition(indicators[1].GetComponent<RectTransform>(), touches.Touches[1]);
    }

    static void setPosition(RectTransform rect, SingleTouch touch) {
        rect.position = new Vector3(touch.Position.x, touch.Position.y);
     }
}
