using System;
using ProjectZ.Core;
using ProjectZ.Game.Entity;
using ProjectZ.Game.Logic;
using ProjectZ.Game.Views;
using ProjectZ.Helpers;
using UnityEngine;

namespace ProjectZ.Game.Controller
{
    public class BoardController
    {
        private IGameLogic _logic;
        private IAnimationManager _animationManager;

        public BoardController(IGameLogic logic, IAnimationManager manager, Transform startPoint)
        {
            BoardViewHelper.SetStartPoint(startPoint);
            InputHandler.Swipe += OnSwipe;
            InputHandler.Tap += OnTap;

            _animationManager = manager;

            _logic = logic;
            _logic.SetController(this);
        }

        public void InitializeViews()
        {
            foreach (var jewel in _logic.Model)
            {
                var view = ObjectPoolManager.Instance.Spawn(ObjectPoolManager.Jewel).GetComponent<JewelView>();
                if (view != null)
                {
                    view.Bind(jewel, _animationManager);
                }
            }
        }

        public void CreateView(Jewel jewel)
        {
            var view = ObjectPoolManager.Instance.Spawn(ObjectPoolManager.Jewel).GetComponent<JewelView>();
            if (view != null)
            {
                view.Bind(jewel, _animationManager);
            }
        }

        public void Execute()
        {
            _logic.Solve();
            _logic.Gravity();
            _logic.Fill();
        }

        public void Teardown()
        {
            InputHandler.Swipe -= OnSwipe;
            InputHandler.Tap -= OnTap;
        }

        private void OnSwipe(JewelView view, Point direction)
        {
            view.Unselect();

            var from = view.Entity;
            var targetPos = new Point(from.Position.x + direction.x, from.Position.y - direction.y);
            Jewel target;
            try
            {
                target = _logic.Model[targetPos.y, targetPos.x];
            }
            catch (IndexOutOfRangeException)
            {
                // ignore
                return;
            }

            _logic.SwapData(from, target);
        }

        private void OnTap(JewelView view)
        {
            if (JewelView.Selected != null)
            {
                var distance = JewelView.Selected.Entity.Position - view.Entity.Position;
                if (Math.Abs(distance.x) + Math.Abs(distance.y) == 1)
                {
                    // Adjacent
                    _logic.SwapData(JewelView.Selected.Entity, view.Entity);
                    view.Unselect();
                    return;
                }
            }
            view.Select();
        }
    }
}