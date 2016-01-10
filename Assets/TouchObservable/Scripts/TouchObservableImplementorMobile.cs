using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Collections.Generic;
using System;

public class TouchObservableImplementorMobile : TouchObservableImplementor
{
    TouchObservable touchObservable;
    IObservable<Unit> updateObservable;

    IDisposable doubleTouchGestureStream;
    IDisposable pinchEnd;
    IDisposable doubleDragEnd;

    public override void Initialize(TouchObservable touchObservable)
    {
        this.touchObservable = touchObservable;
        this.updateObservable = touchObservable.UpdateAsObservable();

        var touchDown = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0));

        var touchUp = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0));

        // SingleDrag
        updateObservable
            .SkipUntil(touchDown)
            .TakeUntil(touchUp)
            .TakeWhile(_ => Input.touchCount == 1)
            .Select(_ => new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            .Buffer(2, 1)
            .RepeatUntilDestroy(touchObservable)
            .Where(touches => touches.Count == 2)
            .Select(touches => touches.Last() - touches.First())
            .Select(delta => new SingleTouch(new Vector2(Input.mousePosition.x, Input.mousePosition.y), delta))
            .Subscribe(touch => SingleDragStream.OnNext(touch));

        var doubleTouch = updateObservable
            .TakeWhile(_ => Input.touchCount == 2)
            .Select(_ => Input.touches)
            .Buffer(2, 1)
            .RepeatUntilDestroy(touchObservable)
            .Where(t => t.Count == 2)
            .Select(touches =>
            {
                var firstFinger = touches.First().First();
                var secondFinger = touches.First().Last();

                var previousFirstFinger = touches.Last().Where(t => t.fingerId == firstFinger.fingerId).First();
                var previousSecondFinger = touches.Last().Where(t => t.fingerId == secondFinger.fingerId).First();

                var firstTouch = new SingleTouch(firstFinger.position, previousFirstFinger.position - firstFinger.position);
                var secondTouch = new SingleTouch(secondFinger.position, previousSecondFinger.position - secondFinger.position);
                return new DoubleTouch(new List<SingleTouch> { firstTouch, secondTouch }, Vector2.Distance(firstFinger.position, secondFinger.position));
            })
            .Subscribe(t =>
            {
                DoubleDragStream.OnNext(t);
            });

        updateObservable
                .Select(_ => Input.touchCount)
                .Buffer(2, 1)
                .Where(c => c.First() == 2 && c.Last() == 1)
                .Subscribe(_ =>
            {
                DoubleDragEndStream.OnNext(Unit.Default);
            });
    }
}
