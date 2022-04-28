using UnityEngine;
using UnityEngine.UI;

namespace ProjectZ.Core
{
    public class AspectRatioFinder : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Vector2 _referenceResolution;
        [SerializeField] private float _defaultHeightSize = 6.4F;

        [Header("Dependencies")] [SerializeField] private CanvasScaler _mainCanvasScaler;

        #endregion

        private const float DefaultPositionZ = -1;

        private float _defaultAspect;
        private float _defaultWidthSize;

        private void Awake()
        {
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            var mainCamera = GetComponent<Camera>();

            if (mainCamera == null)
            {
                mainCamera = Camera.current;
            }

            if (mainCamera == null)
            {
                return;
            }

            _defaultAspect = _referenceResolution.x / _referenceResolution.y;
            _defaultWidthSize = _defaultHeightSize * _defaultAspect;

            var maintainWidth = mainCamera.aspect <= _defaultAspect;

            if (maintainWidth)
            {
                _mainCanvasScaler.matchWidthOrHeight = 0;

                mainCamera.orthographicSize = _defaultWidthSize / mainCamera.aspect;
            }
            else
            {
                _mainCanvasScaler.matchWidthOrHeight = 1;
            }

            var cameraTransform = mainCamera.transform;
            cameraTransform.position = new Vector3(cameraTransform.position.x, mainCamera.orthographicSize, DefaultPositionZ);
        }
    }
}