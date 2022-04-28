using DG.Tweening;
using ProjectZ.Core;
using ProjectZ.Game.Controller;
using ProjectZ.Game.Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectZ.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Inspector

        [Header("Dependencies")] [SerializeField] private Transform _startPoint;
        [SerializeField] private GameObject _gameOverPopup;

        #endregion

        private IAnimationManager _animationManager;
        private BoardController _boardController;

        #region Unity event functions

        private void Awake()
        {
            _animationManager = new AnimationManager();
        }

        private void OnEnable()
        {
            TimerController.Timeout += OnTimeout;

            SceneManager.MoveGameObjectToScene(ObjectPoolManager.Instance.gameObject, SceneManager.GetActiveScene());
        }

        private void OnDisable()
        {
            TimerController.Timeout -= OnTimeout;

            _boardController.Teardown();
        }

        private void Start()
        {
            _boardController = new BoardController(new MatchThreeLogic(8, 8), _animationManager, _startPoint);
        }

        private void Update()
        {
            _boardController.Execute();

            _animationManager.Play();
            _animationManager.Build();
        }

        #endregion

        private void OnTimeout()
        {
            _gameOverPopup.SetActive(true);
            _boardController.Teardown();

            DOVirtual.DelayedCall(3, () => SceneManager.LoadSceneAsync("Home"));
        }
    }
}