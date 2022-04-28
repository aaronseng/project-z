using System.Collections.Generic;
using DG.Tweening;

namespace ProjectZ.Core
{
    public class AnimationManager : IAnimationManager
    {
        private readonly Queue<Tween> _animationQueue = new Queue<Tween>();

        private Sequence _actionSequence;
        private Sequence _positionSequence;
        private Sequence _destroySequence;

        public void EnqueueAction(Tween tween)
        {
            if (_actionSequence == null)
            {
                _actionSequence = DOTween.Sequence().Pause();
            }

            _actionSequence.Insert(0, tween);
        }

        public void EnqueuePosition(Tween tween)
        {
            if (_positionSequence == null)
            {
                _positionSequence = DOTween.Sequence().Pause();
            }

            _positionSequence.Insert(0, tween);
        }

        public void EnqueueDestroy(Tween tween)
        {
            if (_destroySequence == null)
            {
                _destroySequence = DOTween.Sequence().Pause();
            }

            _destroySequence.Insert(0, tween);
        }

        public void Build()
        {
            if (_positionSequence == null && _destroySequence == null && _actionSequence == null)
            {
                return;
            }

            var sequence = DOTween.Sequence().Pause();
            if (_actionSequence != null)
            {
                sequence.Append(_actionSequence);
            }
            if (_destroySequence != null)
            {
                sequence.Append(_destroySequence);
            }
            if (_positionSequence != null)
            {
                sequence.Append(_positionSequence);
            }
            
            _animationQueue.Enqueue(sequence);

            _actionSequence = null;
            _destroySequence = null;
            _positionSequence = null;
        }

        public void Play()
        {
            if (_animationQueue.Count > 0 && !_animationQueue.Peek().IsPlaying())
            {
                _animationQueue.Peek().Play().OnComplete(() => _animationQueue.Dequeue());
            }
        }
    }
}