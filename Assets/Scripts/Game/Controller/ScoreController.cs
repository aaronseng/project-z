using System;
using ProjectZ.Game.Views;
using TMPro;
using UnityEngine;

namespace ProjectZ.Game.Controller
{
    public class ScoreController : MonoBehaviour
    {
        #region Inspector

        [Header("Dependencies")][SerializeField] private TextMeshProUGUI _scoreView;

        #endregion

        #region Unity event functions

        private int _score = 0;

        private void OnEnable()
        {
            JewelView.Destroyed += OnScore;
        }

        private void OnDisable()
        {
            JewelView.Destroyed -= OnScore;
        }

        #endregion

        private void OnScore()
        {
            _score++;

            _scoreView.text = _score.ToString();
        }
    }
}