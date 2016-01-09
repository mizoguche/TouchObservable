using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(TouchObservable))]
public class CameraPositionHandler : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.5f;

    [SerializeField]
    private float zoomSpeed = 0.5f;

    [SerializeField]
    private float swipeTurnSpeed = 0.5f;

    [SerializeField]
    Transform cameraContainerTransform;

    TouchObservable touchObservable;

    void Awake()
    {
        touchObservable = GetComponent<TouchObservable>();
    }

    void Start()
    {
        touchObservable.SingleDrag
            .Select(touch => touch.DeltaPosition)
            .Where(delta => delta != Vector2.zero)
            .Select(delta => cameraContainerTransform.rotation * new Vector3(-delta.x * moveSpeed * 0.05f, 0, -delta.y * moveSpeed * 0.05f))
            .Subscribe(delta => cameraContainerTransform.position += delta);

        touchObservable.Pinch
            .TakeUntil(touchObservable.PinchEnd)
            .Buffer(2, 1)
            .Where(t => t.Count == 2)
            .RepeatUntilDestroy(this)
            .Select(t => t.Last().Distance - t.First().Distance)
            .Subscribe(delta => transform.localPosition -= new Vector3(0, delta * moveSpeed * 0.05f, -delta * moveSpeed * 0.05f));

        touchObservable.DoubleDrag
            .Select(t => t.Touches[0].DeltaPosition)
            .Where(p => p != Vector2.zero)
            .Select(d => new Vector3(Mathf.Clamp(cameraContainerTransform.eulerAngles.x - d.y * swipeTurnSpeed * 0.3f, 0, 65), cameraContainerTransform.eulerAngles.y + d.x * swipeTurnSpeed * 0.3f, 0))
            .Subscribe(e => cameraContainerTransform.eulerAngles = e);
    }
}
