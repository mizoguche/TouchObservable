using UnityEngine;
using UniRx;

[RequireComponent(typeof(TouchObservable))]
public class ObjectTouchHandler : MonoBehaviour
{
    TouchObservable touchObservable;

    void Awake()
    {
        touchObservable = GetComponent<TouchObservable>();
    }

    void Start()
    {
        touchObservable
            .SingleTouchUp
            .Subscribe(touch =>
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.Position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var touchable = hit.collider.gameObject.GetComponent<Touchable>();
                    if (touchable != null)
                    {
                        touchable.OnTouch();
                    }
                }
            });
    }
}
