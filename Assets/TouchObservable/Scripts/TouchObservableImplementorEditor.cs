using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Collections.Generic;

public class TouchObservableImplementorEditor : TouchObservableImplementor
{
    TouchIndicator indicator;

    Vector2 doubleDraggingDelta;

    public override void Initialize(TouchObservable touchObservable)
    {
        var updateObservable = touchObservable.UpdateAsObservable();

        var touchDown = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0));

        var touchUp = touchObservable.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0));

        // SingleDrag
        updateObservable
            .Where(_ => !isDoubleDragKeyHolding())
            .SkipUntil(touchDown)
            .TakeUntil(touchUp)
            .Select(_ => new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            .Buffer(2, 1)
            .RepeatUntilDestroy(touchObservable)
            .Where(touches => touches.Count == 2)
            .Select(touches => touches.Last() - touches.First())
            .Select(delta => new SingleTouch(new Vector2(Input.mousePosition.x, Input.mousePosition.y), delta))
            .Subscribe(touch => SingleDragStream.OnNext(touch));

        // Double drag
        updateObservable
            .Where(_ => isDoubleDragKeyDown())
            .Subscribe(_ =>
            {
                var xDelta = (Screen.width - Input.mousePosition.x) - Input.mousePosition.x;
                var yDelta = (Screen.height - Input.mousePosition.y) - Input.mousePosition.y;
                doubleDraggingDelta = new Vector2(xDelta, yDelta);
                var indicatorObject = GameObject.Instantiate(Resources.Load("Prefabs/TouchIndicator")) as GameObject;
                indicator = indicatorObject.GetComponent<TouchIndicator>();
            });

        var doubleDragging = updateObservable
            .Where(_ => isDoubleDragKeyHolding());
        doubleDragging
            .Subscribe(_ =>
            {
                var touch1 = new SingleTouch(Input.mousePosition, Input.mouseScrollDelta);
                var touch2 = new SingleTouch(new Vector2(Input.mousePosition.x + doubleDraggingDelta.x, Input.mousePosition.y + doubleDraggingDelta.y), Input.mouseScrollDelta + doubleDraggingDelta);
                var touches = new DoubleTouch(new List<SingleTouch> { touch1, touch2 }, Vector2.Distance(touch1.Position, touch2.Position));
                indicator.SetPosition(touches);
            });
        doubleDragging
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
                DoubleDragStream.OnNext(touches);
            });
        doubleDragging
            .Zip(touchUp, (l, r) => Unit.Default)
            .Subscribe(_ => DoubleDragEndStream.OnNext(Unit.Default));

        updateObservable
            .Where(_ => isDoubleDragKeyUp())
            .Subscribe(_ =>
            {
                GameObject.Destroy(indicator.gameObject);
            });
    }

    bool isDoubleDragKeyDown()
    {
        return Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
    }

    bool isDoubleDragKeyHolding()
    {
        return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
    }

    bool isDoubleDragKeyUp()
    {
        return Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt);
    }
}
