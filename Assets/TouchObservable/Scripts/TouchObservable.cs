using UniRx;
using UnityEngine;

public class TouchObservable : MonoBehaviour
{
    TouchObservableImplementor impl = new TouchObservableImplementorEditor();

    public IObservable<SingleTouch> SingleTouchUp { get { return impl.SingleTouchUp; } }

    void Start() { impl.Initialize(this); }
}
