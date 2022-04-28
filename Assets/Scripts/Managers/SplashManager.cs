using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectZ.Managers
{
    /// <summary>
    /// We make all device initializations such as input manager, audio middleware and the device specific configurations in splash scene.
    /// Heavy object pool creations and Network connections are also good candidate to be done in splash scene as well as CDN updates
    /// for this study case I'll just implement a pseudo initialization which will delay a second then load the main scene.
    /// </summary>
    public class SplashManager : MonoBehaviour
    {
        #region Inspector

        [Header("Dependencies")][SerializeField] private Image _progressBar;

        #endregion

        #region Unity event functions

        private void Start()
        {
            _ = InitializeAsync();
        }

        #endregion

        #region Manager Logic

        /// <summary>
        /// Pseudo Initialization for Splash screen. This method will fill the progressbar in 2 seconds then load the Main scene asynchronously.
        /// </summary>
        private async Task InitializeAsync()
        {
            float total = 2;
            float elapsed = 0;

            // Simulate 1 second delay
            while(elapsed < total)
            {
                await Task.Yield();
                elapsed += Time.deltaTime;

                _progressBar.fillAmount = elapsed / total;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Home");

            // Wait until the asynchronous scene fully loaded
            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }

        #endregion
    }
}