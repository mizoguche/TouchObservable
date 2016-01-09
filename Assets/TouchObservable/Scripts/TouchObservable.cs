using UniRx;
using UnityEngine;

public class TouchObservable : MonoBehaviour
{
    #if UNITY_IOS || UNITY_ANDROID
    TouchObservableImplementor impl = new TouchObservableImplementorMobile();
    #else
    TouchObservableImplementor impl = new TouchObservableImplementorEditor();
    #endif

    public IObservable<SingleTouch> SingleDrag { get { return impl.SingleDrag; } }

    public IObservable<DoubleTouch> Pinch { get { return impl.Pinch; } }
    public IObservable<Unit> PinchEnd { get { return impl.PinchEnd; } }

    public IObservable<DoubleTouch> DoubleDrag { get { return impl.DoubleDrag; } }
    public IObservable<Unit> DoubleDragEnd { get { return impl.DoubleDragEnd; } }

    void Start() { impl.Initialize(this); }
}
