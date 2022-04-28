using UnityEngine;

namespace ProjectZ.Helpers
{
    public static class BoardViewHelper
    {
        private const float JewelWidth = 0.92f;
        private const float JewelHeight = 0.92f; 

        private static Vector3 _startPoint;

        public static void SetStartPoint(Transform startPoint)
        {
            _startPoint = startPoint.position;
        }
        
        public static Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(_startPoint.x + JewelWidth * x, _startPoint.y - JewelHeight * y);
        }
    }
}