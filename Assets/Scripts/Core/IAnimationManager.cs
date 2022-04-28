using DG.Tweening;

namespace ProjectZ.Core
{
    public interface IAnimationManager
    {
        void EnqueueAction(Tween tween);
        void EnqueuePosition(Tween tween);
        void EnqueueDestroy(Tween tween);
        void Build();
        void Play();
    }
}