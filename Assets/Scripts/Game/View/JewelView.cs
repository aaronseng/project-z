using System;
using DG.Tweening;
using ProjectZ.Core;
using ProjectZ.Game.Entity;
using ProjectZ.Helpers;
using UnityEngine;

namespace ProjectZ.Game.Views
{
    public class JewelView : MonoBehaviour, IPositionListener, IDestroyedListener
    {
        #region Inspector

        [SerializeField] private Sprite[] _colors;

        [Header("Dependencies")] [SerializeField] private SpriteRenderer _renderer;

        #endregion

        public static event Action Destroyed;

        private IAnimationManager _animationManager;

        public static JewelView Selected { get; private set; }

        public Jewel Entity { get; private set; }

        #region Unity event functions

        private void OnDisable()
        {
            _renderer.color = Color.white;
            transform.localScale = Vector3.one;

            if (Entity == null) return;

            Entity.RemoveDestroyedListener(this);
            Entity.RemovePositionListener(this);
        }

        #endregion

        #region View Logic

        public void Bind(Jewel entity, IAnimationManager animationManager)
        {
            Entity = entity;
            _animationManager = animationManager;

            _renderer.sprite = _colors[entity.Color];
            transform.localPosition = BoardViewHelper.GetWorldPosition(Entity.Position.x, Entity.Position.y);

            entity.AddDestroyedListener(this);
            entity.AddPositionListener(this);
        }

        public void Select()
        {
            if (Selected)
                Selected.Unselect();

            _renderer.color = Color.gray;
            Selected = this;
        }

        public void Unselect()
        {
            if (Selected == null) return;

            Selected._renderer.color = Color.white;
            Selected = null;
        }

        #endregion

        #region Entity observers

        public void OnPosition(Point position)
        {
            var tween = transform.DOMove(BoardViewHelper.GetWorldPosition(position.x, position.y), 0.3f).Pause();
            if (Entity.IsSwapped)
            {
                _animationManager.EnqueueAction(tween);
            }
            else
            {
                _animationManager.EnqueuePosition(tween);
            }
        }

        public void OnDestroyed(bool destroyed)
        {
            var sequence = DOTween.Sequence().Pause();
            sequence.Append(transform.DOScale(1.3f, 0.3f));
            sequence.Join(_renderer.DOFade(0, 0.3f));
            sequence.AppendCallback(() =>
            {
                Destroyed?.Invoke();
                ObjectPoolManager.Instance.DeSpawn(ObjectPoolManager.Jewel, gameObject);
            });

            _animationManager.EnqueueDestroy(sequence);
        }

        #endregion
    }
}