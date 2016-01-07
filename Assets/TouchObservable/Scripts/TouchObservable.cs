using UniRx;
using UnityEngine;

public class TouchObservable : MonoBehaviour
{
    TouchObservableImplementor impl = new TouchObservableImplementorEditor();

    public IObservable<SingleTouch> SingleDrag { get { return impl.SingleDrag; } }

    public IObservable<DoubleTouch> Pinch { get { return impl.Pinch; } }
    public IObservable<Unit> PinchEnd { get { return impl.PinchEnd; } }

    void Start() { impl.Initialize(this); }
}
