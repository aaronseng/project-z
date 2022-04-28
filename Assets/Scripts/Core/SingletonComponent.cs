using UnityEngine;

namespace ProjectZ.Core
{
    /// <summary>
    /// Classes derived from this class become singleton.
    /// </summary>
    public abstract class SingletonComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance = null;

        /// <summary>
        /// Returns the singleton instance of the component
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var singletonObj = new GameObject();
                        singletonObj.name = typeof(T).ToString();
                        _instance = singletonObj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        #region Unity Methods

        protected virtual void Awake()
        {
            // if we try to create second instance of a Singleton component throw an exception.
            if (Instance != null && Instance != this)
            {
                throw new System.Exception("Singleton instance is already exists.");
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        #endregion
    }
}