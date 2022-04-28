using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ProjectZ.Game.Controller
{
    public class TimerController : MonoBehaviour
    {
        #region Inspector

        [Header("Config")] [SerializeField] private int _roundTime = 60; 
        [Header("Dependencies")][SerializeField] private TextMeshProUGUI _timerView;

        #endregion

        public static event Action Timeout;

        private int _elapsed;

        private void Start()
        {
            _ = Stopwatch();
        }

        private async Task Stopwatch()
        {
            while (_elapsed < _roundTime)
            {
                await Task.Delay(1000);
                _timerView.text = (_roundTime - ++_elapsed).ToString("");
            }

            Timeout?.Invoke();
        }
    }
}