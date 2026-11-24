using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meatgrinder.Presentation
{
    [AddComponentMenu("")]
    public sealed class MenuButton : UnityEngine.UI.Button
    {
        static readonly Color Rest = new Color(.12f, .13f, .14f, .98f);
        static readonly Color Hover = new Color(.20f, .17f, .15f);
        static readonly Color Selected = new Color(.26f, .17f, .12f);
        static readonly Color Pressed = new Color(.38f, .19f, .10f);
        static readonly Color Disabled = new Color(.09f, .095f, .10f);
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
                SelectionState.Highlighted => Hover,
                SelectionState.Selected => Selected,
                SelectionState.Pressed => Pressed,
                SelectionState.Disabled => Disabled,
                _ => Rest
            }, blend);
            var edge = GameUi.Accent;
            edge.a = state == SelectionState.Disabled ? 0 : state == SelectionState.Normal ? .22f : 1;
            rail.color = Color.Lerp(rail.color, edge, blend);
            rail.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                state == SelectionState.Selected ? 8 : state == SelectionState.Highlighted ? 6 : 4);
            caption.color = state == SelectionState.Disabled ? new Color(.56f, .56f, .55f) : GameUi.Paper;
            float offset = state == SelectionState.Pressed ? -3 : IsFocus(state) ? 4 : 0;
            motion = reduced ? 0 : Mathf.Lerp(motion, offset, blend);
            face.rectTransform.anchoredPosition = new Vector2(motion, 0);
        }

        static bool IsFocus(SelectionState value) => value == SelectionState.Highlighted || value == SelectionState.Selected;
    }
}
