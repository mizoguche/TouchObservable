using UniRx;
using UnityEngine;

public abstract class TouchObservableImplementor
{
    public abstract void Initialize(MonoBehaviour monoBehaviour);

    protected Subject<SingleTouch> SingleDragStream = new Subject<SingleTouch>();
    public IObservable<SingleTouch> SingleDrag { get { return SingleDragStream.AsObservable(); } }
}
