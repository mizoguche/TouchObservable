using UnityEngine;
using UniRx;
using UniRx.Triggers;

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
    }
}
