using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectZ.Managers
{
    /// <summary>
    /// MainManager just handles Main menu button events.
    /// </summary>
    public class HomeManager : MonoBehaviour
    {
        #region Manager Logic

        /// <summary>
        /// Event Handler for the Start button clicked event. 
        /// We load the Loading Scene which handles the game play related data before starting the game itself.
        /// </summary>
        public async void OnStartClicked()
        {
            await LoadLoadingSceneAsync();
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void OnQuitClicked()
        {
            Application.Quit();
        }

        /// <summary>
        /// Loads the LoadingScene asynchronously.
        /// </summary>
        private async Task LoadLoadingSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading");

            // Wait until the asynchronous scene fully loaded
            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }

        #endregion
    }
}