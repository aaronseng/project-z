using System.Collections.Generic;
using ProjectZ.Core;

namespace ProjectZ.Game.Entity
{
    public class Jewel
    {
        private readonly List<IDestroyedListener> _destroyedListeners = new List<IDestroyedListener>();
        private readonly List<IPositionListener> _positionListeners = new List<IPositionListener>();

        private bool _destroyed;
        private Point _position;

        public Point Position
        {
            get => _position;
            set
            {
                if (_position == value) return;

                _position = value;
                foreach (var listener in _positionListeners)
                {
                    listener.OnPosition(_position);
                }
            }
        }

        public bool Destroyed
        {
            get => _destroyed;
            set
            {
                if (_destroyed == value) return;

                _destroyed = value;
                foreach (var listener in _destroyedListeners)
                {
                    listener.OnDestroyed(_destroyed);
                }
            }
        }

        public int Color { get; }

        public bool IsDirty { get; set; }

        public bool IsSwapped { get; set; }

        public Jewel(Point position, int color)
        {
            _position = position;
            Color = color;
        }

        public void AddPositionListener(IPositionListener listener)
        {
            _positionListeners.Add(listener);
        }

        public void AddDestroyedListener(IDestroyedListener listener)
        {
            _destroyedListeners.Add(listener);
        }

        public void RemovePositionListener(IPositionListener listener)
        {
            _positionListeners.Remove(listener);
        }

        public void RemoveDestroyedListener(IDestroyedListener listener)
        {
            _destroyedListeners.Remove(listener);
        }
    }
}