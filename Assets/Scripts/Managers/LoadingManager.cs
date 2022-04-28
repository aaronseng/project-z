using System.Threading.Tasks;
using ProjectZ.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectZ.Managers
{
    /// <summary>
    /// We make all light-weight object pool creations, loading terrains, gameplay related multiplayer network connections,
    /// etc. in the LoadingScene, also game user interface preparations are good candidate to be done in the LoadingScene as well.
    /// For this study case I'll just implement light-weight object pool initialization in LoadingScene.
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        #region Inspector

        [Header("Dependencies")][SerializeField] private Image _progressBar;

        #endregion

        private bool _isGameLoading = false;

        #region Unity event functions

        private async void Update()
        {
            // Update the progressbar with the current object pool progress.
            _progressBar.fillAmount = ObjectPoolManager.Instance.Progress;

            // Load the game scene when object pool is ready.
            if (ObjectPoolManager.Instance.IsReady && !_isGameLoading)
            {
                _isGameLoading = true;
                await LoadGameSceneAsync();
            }
        }

        #endregion

        #region Manager Logic

        /// <summary>
        /// Loads the GameScene asynchronously.
        /// </summary>
        private async Task LoadGameSceneAsync()
        {
            DontDestroyOnLoad(ObjectPoolManager.Instance.gameObject);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }

        #endregion
    }
}