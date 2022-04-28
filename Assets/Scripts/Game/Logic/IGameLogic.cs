using ProjectZ.Game.Controller;
using ProjectZ.Game.Entity;

namespace ProjectZ.Game.Logic
{
    public interface IGameLogic
    {
        public Jewel[,] Model { get; }

        public void SetController(BoardController controller);
        public void SwapData(Jewel from, Jewel target);
        public void Solve();
        public void Gravity();
        public void Fill();
    }
}