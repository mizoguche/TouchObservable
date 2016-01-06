using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class TouchObservableImplementorEditor : TouchObservableImplementor
{
    public override void Initialize(MonoBehaviour monoBehaviour)
    {
        var updateObservable = monoBehaviour.UpdateAsObservable();

        // SingleTouchUp
        updateObservable
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => new SingleTouch(Input.mousePosition, Vector2.zero))
            .Subscribe(touch => SingleTouchUpStream.OnNext(touch));


        var touchDown = monoBehaviour.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0));

        var touchUp = monoBehaviour.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0));

        // SingleDrag
        updateObservable
            .SkipUntil(touchDown)
            .TakeUntil(touchUp)
            .Select(_ => Input.mousePosition)
            .Buffer(2, 1)
            .RepeatUntilDestroy(monoBehaviour)
            .Where(touches => touches.Count == 2)
            .Select(touches => touches.Last() - touches.First())
            .Select(delta => new SingleTouch(Input.mousePosition, delta))
            .Subscribe(touch => SingleDragStream.OnNext(touch));
    }
}
