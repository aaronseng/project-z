using System;
using System.Collections.Generic;
using System.Linq;
using ProjectZ.Core;
using ProjectZ.Game.Controller;
using ProjectZ.Game.Entity;
using ProjectZ.Helpers;
using Random = UnityEngine.Random;

namespace ProjectZ.Game.Logic
{
    public class MatchThreeLogic : IGameLogic
    {
        private const int ColorCount = 5;
        private const int MinSolutionCount = 3;

        private readonly Point[] _horizontal = {new Point(1, 0), new Point(-1, 0)};
        private readonly Point[] _vertical = {new Point(0, 1), new Point(0, -1)};
        private readonly HashSet<int> _gravityColumns = new HashSet<int>();
        
        private bool _isModelDirty;
        private BoardController _controller;

        public Jewel[,] Model { get; private set; }

        public MatchThreeLogic(int width, int height)
        {
            InitializeBoard(width, height);
        }

        #region Initialize

        /// <summary>
        /// Initialize a board with the given size
        /// </summary>
        /// <param name="width">Width of the board</param>
        /// <param name="height">Height of the board</param>
        private void InitializeBoard(int width, int height)
        {
            Model = new Jewel[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Model[row, col] = CreateJewel(col, row);
                }
            }
        }

        /// <summary>
        /// Creates a jewel entity at the given position which is not repetitive with adjacent jewels 
        /// </summary>
        /// <param name="x">Position x</param>
        /// <param name="y">Position y</param>
        /// <returns>The jewel entity</returns>
        private Jewel CreateJewel(int x, int y)
        {
            List<int> excluded = new List<int>(2);

            var left = x - 1;
            var top = y - 1;
            if (left >= 0)
            {
                excluded.Add(Model[y, left].Color);
            }

            if (top >= 0)
            {
                excluded.Add(Model[top, x].Color);
            }

            return new Jewel(new Point(x, y), Util.RandomExceptList(ColorCount, excluded));
        }

        #endregion

        #region Logic

        public void SetController(BoardController controller)
        {
            _controller = controller;

            _controller.InitializeViews();
        }

        public void SwapData(Jewel from, Jewel target)
        {
            // Update model
            Model[target.Position.y, target.Position.x] = from;
            Model[from.Position.y, from.Position.x] = target;

            // Update swapped
            from.IsSwapped = true;
            target.IsSwapped = true;
            
            // Update Entity data which will trigger Views observer
            var temp = from.Position;
            from.Position = target.Position;
            target.Position = temp;

            // Dirty those entities
            from.IsDirty = true;
            target.IsDirty = true;

            // Dirty model
            _isModelDirty = true;
        }

        public void Solve()
        {
            if (!_isModelDirty) return;

            bool needRollback = true;
            var dirtyEntities = Model.Cast<Jewel>().Where(jewel => jewel.IsDirty).ToList();

            foreach (var jewel in dirtyEntities)
            {
                bool horizontalFound = FindStraightSolution(jewel, _horizontal, out var horizontalSolution);
                bool verticalFound = FindStraightSolution(jewel, _vertical, out var verticalSolution);

                if (horizontalFound)
                {
                    foreach (var entity in horizontalSolution)
                    {
                        entity.Destroyed = true;
                        _gravityColumns.Add(entity.Position.x);
                    }
                }

                if (verticalFound)
                {
                    foreach (var entity in verticalSolution)
                    {
                        entity.Destroyed = true;
                    }
                    _gravityColumns.Add(verticalSolution[0].Position.x);
                }

                needRollback &= (jewel.IsSwapped && !verticalFound && !horizontalFound);

                jewel.IsDirty = false;
                jewel.IsSwapped = false;
            }

            if (needRollback)
            {
                if (dirtyEntities.Count != 2)
                {
                    throw new Exception("Entities swapped but expected size is different");
                }
                RollbackData(dirtyEntities[0], dirtyEntities[1]);
            }

            _isModelDirty = false;
        }

        public void Gravity()
        {
            if (_gravityColumns.Count == 0) return;

            foreach (var column in _gravityColumns)
            {
                int gravity = 0;
                int height = Model.GetLength(0) - 1;
                for (int row = height; row >= 0; row--)
                {
                    var jewel = Model[row, column];
                    if (jewel.Destroyed)
                    {
                        gravity++;

                        Model[jewel.Position.y, jewel.Position.x] = null;
                    }
                    else if (gravity > 0 && !jewel.Destroyed)
                    {
                        var newPos = jewel.Position + new Point(0, gravity);

                        Model[jewel.Position.y, jewel.Position.x] = null;
                        Model[newPos.y, newPos.x] = jewel;
                        jewel.Position = newPos;
                        jewel.IsDirty = true;
                    }
                }
            }
        }

        public void Fill()
        {
            if (_gravityColumns.Count == 0) return;

            foreach (var column in _gravityColumns)
            {
                int generatorPosY = -1;
                int height = Model.GetLength(0) - 1;
                for (int row = height; row >= 0; row--)
                {
                    var jewel = Model[row, column];

                    if (jewel != null) continue;

                    jewel = new Jewel(new Point(column, generatorPosY--), Random.Range(0, ColorCount));
                    Model[row, column] = jewel;

                    _controller.CreateView(jewel);

                    jewel.Position = new Point(column, row);
                    jewel.IsDirty = true;
                }
            }

            _gravityColumns.Clear();
            _isModelDirty = true;
        }

        private bool FindStraightSolution(Jewel jewel, Point[] directions, out List<Jewel> solution)
        {
            solution = new List<Jewel>();
            var current = jewel;

            solution.Add(current);

            foreach (var direction in directions)
            {
                while (IsAdjacentMatched(current, direction, out Jewel adjacent))
                {
                    solution.Add(adjacent);
                    current = adjacent;
                }

                current = jewel;
            }

            return solution.Count >= MinSolutionCount;
        }

        private bool IsAdjacentMatched(Jewel from, Point direction, out Jewel adjacent)
        {
            adjacent = null;
            var targetPos = from.Position + direction;
            try
            {
                adjacent = Model[targetPos.y, targetPos.x];
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

            return adjacent.Color == from.Color;
        }

        private void RollbackData(Jewel from, Jewel target)
        {
            // Update model
            Model[target.Position.y, target.Position.x] = from;
            Model[from.Position.y, from.Position.x] = target;

            // Update Entity data which will trigger Views observer
            var temp = from.Position;
            from.Position = target.Position;
            target.Position = temp;
        }

        #endregion
    }
}