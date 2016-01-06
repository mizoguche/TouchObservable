using UniRx;
using UnityEngine;

public class TouchObservable : MonoBehaviour
{
    TouchObservableImplementor impl = new TouchObservableImplementorEditor();

    public IObservable<SingleTouch> SingleDrag { get { return impl.SingleDrag; } }

    void Start() { impl.Initialize(this); }
}
