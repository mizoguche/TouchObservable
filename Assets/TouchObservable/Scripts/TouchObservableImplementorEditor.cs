using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Collections.Generic;

public class TouchObservableImplementorEditor : TouchObservableImplementor
{
    TouchIndicator indicator;

    public override void Initialize(TouchObservable touchObservable)
    {
        var updateObservable = touchObservable.UpdateAsObservable();

        var touchDown = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0));

        var touchUp = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0));

        // SingleDrag
        updateObservable
            .Where(_ => !isPinchOptionKeyHolding())
            .SkipUntil(touchDown)
            .TakeUntil(touchUp)
            .Select(_ => new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            .Buffer(2, 1)
            .RepeatUntilDestroy(touchObservable)
            .Where(touches => touches.Count == 2)
            .Select(touches => touches.Last() - touches.First())
            .Select(delta => new SingleTouch(new Vector2(Input.mousePosition.x, Input.mousePosition.y), delta))
            .Subscribe(touch => SingleDragStream.OnNext(touch));

        // Pinch
        updateObservable
            .Where(_ => isPinchOptionKeyDown())
            .Subscribe(_ =>
            {
                var indicatorObject = GameObject.Instantiate(Resources.Load("Prefabs/TouchIndicator")) as GameObject;
                indicator = indicatorObject.GetComponent<TouchIndicator>();
            }); ;
        var pinching = updateObservable
            .Where(_ => isPinchOptionKeyHolding());
        pinching
            .Subscribe(_ =>
            {
                var touch1 = new SingleTouch(Input.mousePosition, Input.mouseScrollDelta);
                var touch2 = new SingleTouch(new Vector3(Screen.width, Screen.height) - Input.mousePosition, new Vector2(Screen.width, Screen.height) - Input.mouseScrollDelta);
                var touches = new DoubleTouch(new List<SingleTouch> { touch1, touch2 }, Vector2.Distance(touch1.Position, touch2.Position));
                indicator.SetPosition(touches);
            });
        pinching
            .SkipUntil(touchDown)
            .TakeUntil(touchUp)
            .Select(_ => new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            .Buffer(2, 1)
            .Where(touches => touches.Count == 2)
            .RepeatUntilDestroy(touchObservable)
            .Select(touches => touches.Last() - touches.First())
            .Select(delta => new SingleTouch(new Vector2(Input.mousePosition.x, Input.mousePosition.y), delta))
            .Subscribe(touch1 =>
            {
                var touch2 = new SingleTouch(new Vector2(Screen.width, Screen.height) - touch1.Position, new Vector2(Screen.width, Screen.height) - touch1.DeltaPosition);
                var touches = new DoubleTouch(new List<SingleTouch> { touch1, touch2 }, Vector2.Distance(touch1.Position, touch2.Position));
                PinchStream.OnNext(touches);
            });
        pinching
            .Zip(touchUp, (l, r) => Unit.Default)
            .Subscribe(_ => PinchEndStream.OnNext(Unit.Default));
        updateObservable
            .Where(_ => isPinchOptionKeyUp())
            .Subscribe(_ =>
            {
                GameObject.Destroy(indicator.gameObject);
            }); ;
    }

    bool isPinchOptionKeyDown()
    {
        return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
    }

    bool isPinchOptionKeyHolding()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }

    bool isPinchOptionKeyUp()
    {
        return Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt);
    }
}
