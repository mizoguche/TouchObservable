using UniRx;
using UnityEngine;

public class TouchObservable : MonoBehaviour
{
    #if UNITY_EDITOR
    TouchObservableImplementor impl = new TouchObservableImplementorEditor();
    #else
    TouchObservableImplementor impl = new TouchObservableImplementorMobile();
    #endif

    public IObservable<SingleTouch> SingleDrag { get { return impl.SingleDrag; } }

    public IObservable<DoubleTouch> DoubleDrag { get { return impl.DoubleDrag; } }
    public IObservable<Unit> DoubleDragEnd { get { return impl.DoubleDragEnd; } }

    void Start() { impl.Initialize(this); }
}
