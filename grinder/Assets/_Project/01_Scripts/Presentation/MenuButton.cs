using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meatgrinder.Presentation
{
    [AddComponentMenu("")]
    public sealed class MenuButton : UnityEngine.UI.Button
    {
        UnityEngine.UI.Image face;
        UnityEngine.UI.Image rail;
        UnityEngine.UI.Text caption;
        Func<bool> reduceMotion;
        Action onFocus;
        SelectionState state;
        float motion;

        internal void Configure(UnityEngine.UI.Image face, UnityEngine.UI.Image rail, UnityEngine.UI.Text caption,
            Func<bool> reduceMotion, Action onFocus)
        {
            this.face = face;
            this.rail = rail;
            this.caption = caption;
            this.reduceMotion = reduceMotion;
            this.onFocus = onFocus;
            DoStateTransition(currentSelectionState, true);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (IsActive() && IsInteractable()) DoStateTransition(SelectionState.Pressed, true);
            base.OnSubmit(eventData);
        }

        protected override void DoStateTransition(SelectionState next, bool instant)
        {
            if (face == null) return;
            bool enteredFocus = IsFocus(next) && !IsFocus(state) && state != SelectionState.Pressed;
            state = next;
            if (instant || next == SelectionState.Pressed) Render(1);
            if (enteredFocus && !instant) onFocus?.Invoke();
        }

        void Update() => Render(Mathf.Clamp01(Time.unscaledDeltaTime * 22));

        void Render(float blend)
        {
            if (face == null) return;
            bool reduced = reduceMotion?.Invoke() == true;
            if (reduced) blend = 1;
            face.color = Color.Lerp(face.color, state switch
            {
                SelectionState.Highlighted => GameUi.Hover,
                SelectionState.Selected => GameUi.Blood,
                SelectionState.Pressed => GameUi.Pressed,
                SelectionState.Disabled => GameUi.Ink,
                _ => GameUi.Charcoal
            }, blend);
            var edge = GameUi.Accent;
            edge.a = state == SelectionState.Disabled ? 0 : state == SelectionState.Normal ? .22f : 1;
            rail.color = Color.Lerp(rail.color, edge, blend);
            rail.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                state == SelectionState.Selected ? 8 : state == SelectionState.Highlighted ? 6 : 4);
            caption.color = state == SelectionState.Disabled ? GameUi.Muted : GameUi.Paper;
            float offset = state == SelectionState.Pressed ? -3 : IsFocus(state) ? 4 : 0;
            motion = reduced ? 0 : Mathf.Lerp(motion, offset, blend);
            face.rectTransform.anchoredPosition = new Vector2(motion, 0);
        }

        static bool IsFocus(SelectionState value) => value == SelectionState.Highlighted || value == SelectionState.Selected;
    }
}
