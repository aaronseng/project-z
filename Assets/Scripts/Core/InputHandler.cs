using System;
using System.Collections.Generic;
using Lean.Touch;
using ProjectZ.Game.Views;
using UnityEngine;

namespace ProjectZ.Core
{
    public class InputHandler : MonoBehaviour
    {
        public static event Action<JewelView> Tap;
        public static event Action<JewelView, Point> Swipe;

        private const float DirectionThreshold = 0.5f;

        private readonly List<Vector2> _cardinals = new List<Vector2>(4) {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

        private bool _isSwiping;
        private Camera _mainCamera;

        private void OnEnable()
        {
            _mainCamera = Camera.main;

            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerUpdate += OnFingerUpdate;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerUpdate -= OnFingerUpdate;
            
        }

        private void OnFingerUpdate(LeanFinger finger)
        {
            if (finger.SwipeScreenDelta.magnitude * LeanTouch.ScalingFactor > LeanTouch.Instance.SwipeThreshold && finger.Age <= LeanTouch.Instance.TapThreshold && !_isSwiping)
            {
                _isSwiping = true;
                Point swipeDirection = new Point(0, 0);
                foreach (var direction in _cardinals)
                {
                    if (!IsDirection(finger.SwipeScreenDelta.normalized, direction)) continue;

                    swipeDirection.x = (int) direction.x;
                    swipeDirection.y = (int) direction.y;
                }

                var view = GetJewelView(finger);
                if (view == null) return;

                Swipe?.Invoke(view, swipeDirection);
            }
        }

        private void OnFingerUp(LeanFinger obj)
        {
            _isSwiping = false;
        }

        private void OnFingerTap(LeanFinger finger)
        {
            var view = GetJewelView(finger);
            if (view == null) return;

            Tap?.Invoke(view);
        }

        private JewelView GetJewelView(LeanFinger finger)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenPointToRay(finger.StartScreenPosition).origin, Vector2.zero);

            return hit.transform == null ? null : hit.transform.GetComponent<JewelView>();
        }

        private static bool IsDirection(Vector2 direction, Vector2 cardinal)
        {
            return Vector2.Dot(direction, cardinal) > DirectionThreshold;
        }
    }
}