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
            .Where(delta => delta != Vector3.zero)
            .Select(delta => new Vector3(-delta.x * moveSpeed * 0.05f, 0, -delta.y * moveSpeed * 0.05f))
            .Subscribe(delta => cameraContainerTransform.position += delta);
    }
}
