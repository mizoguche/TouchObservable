using UniRx;
using UnityEngine;

public abstract class TouchObservableImplementor
{
    public abstract void Initialize(TouchObservable touchObservable);

    protected Subject<SingleTouch> SingleDragStream = new Subject<SingleTouch>();
    public IObservable<SingleTouch> SingleDrag { get { return SingleDragStream.AsObservable(); } }

    protected Subject<DoubleTouch> PinchStream = new Subject<DoubleTouch>();
    public IObservable<DoubleTouch> Pinch { get { return PinchStream.AsObservable(); } }
    protected Subject<Unit> PinchEndStream = new Subject<Unit>();
    public IObservable<Unit> PinchEnd { get { return PinchEndStream.AsObservable(); } }

    protected Subject<DoubleTouch> DoubleDragStream = new Subject<DoubleTouch>();
    public IObservable<DoubleTouch> DoubleDrag { get { return DoubleDragStream.AsObservable(); } }
    protected Subject<Unit> DoubleDragEndStream = new Subject<Unit>();
    public IObservable<Unit> DoubleDragEnd { get { return DoubleDragEndStream.AsObservable(); } }
}
