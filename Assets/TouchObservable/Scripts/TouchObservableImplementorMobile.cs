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
    }
}
