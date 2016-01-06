﻿using UniRx;
using UnityEngine;

public abstract class TouchObservableImplementor
{
    public abstract void Initialize(MonoBehaviour monoBehaviour);
    protected Subject<SingleTouch> SingleTouchUpStream = new Subject<SingleTouch>();
    public IObservable<SingleTouch> SingleTouchUp { get { return SingleTouchUpStream.AsObservable(); } }

    protected Subject<SingleTouch> SingleDragStream = new Subject<SingleTouch>();
    public IObservable<SingleTouch> SingleDrag { get { return SingleDragStream.AsObservable(); } }
}
