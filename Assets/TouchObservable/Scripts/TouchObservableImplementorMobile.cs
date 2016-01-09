using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Collections.Generic;

public class TouchObservableImplementorMobile : TouchObservableImplementor
{
    public override void Initialize(TouchObservable touchObservable)
    {
        var updateObservable = touchObservable.UpdateAsObservable();

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

        // Pinch
        var pinching = updateObservable
            .TakeWhile(_ => Input.touchCount == 2)
            .Select(_ => Input.touches);

        pinching
            .Buffer(2, 1)
            .Where(t => t.Count == 2)
            .RepeatUntilDestroy(touchObservable)
            .Subscribe(touches =>
            {
                var firstFinger = touches.First().First();
                var secondFinger = touches.First().Last();

                var previousFirstFinger = touches.Last().Where(t => t.fingerId == firstFinger.fingerId).First();
                var previousSecondFinger = touches.Last().Where(t => t.fingerId == secondFinger.fingerId).First();

                var firstTouch = new SingleTouch(firstFinger.position, firstFinger.position - previousFirstFinger.position);
                var secondTouch = new SingleTouch(secondFinger.position, secondFinger.position - previousSecondFinger.position);
                var doubleTouch = new DoubleTouch(new List<SingleTouch>{firstTouch, secondTouch}, Vector2.Distance(firstFinger.position, secondFinger.position));

                PinchStream.OnNext(doubleTouch);
            });

        updateObservable
            .Select(_ => Input.touchCount)
            .Buffer(2, 1)
            .Where(c => c.First() == 2 && c.Last() == 1)
            .Subscribe(_ => PinchEndStream.OnNext(Unit.Default));
    }
}
