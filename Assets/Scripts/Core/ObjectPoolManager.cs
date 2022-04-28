using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Core
{
    /// <summary>
    /// Object Pool Manager.
    /// </summary>
    public class ObjectPoolManager : SingletonComponent<ObjectPoolManager>
    {
        public const string Jewel = "JEWEL";

        /// <summary>
        /// Helper class for keeping pool configs for the object pool and exposing it to Unity inspector.
        /// </summary>
        [Serializable]
        public class Pool
        {
            public string key;
            public GameObject gameObject;
            public int count;
        }

        [SerializeField] private List<Pool> _pools = new List<Pool>();

        /// <summary>
        /// When Progress reaches to 1.0 all default objects pools are initialized and ready to use.
        /// </summary>
        public float Progress { get; private set; }

        public bool IsReady => Math.Abs(Progress - 1.0f) < 9.999999439624929E-11;

        private Dictionary<string, Queue<GameObject>> _objPools = new Dictionary<string, Queue<GameObject>>();

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            // While initializing pools with default size for each given pair it's nice to not block the Unity's main thread
            StartCoroutine(InitializePools());
        }

        #endregion

        #region ObjectPoolManager Logic

        /// <summary>
        /// Takes away a pooled object from the pool.
        /// </summary>
        /// <param name="pool"> Pool name </param>
        /// <param name="active"> State of the spawned object </param>
        /// <returns> Pooled object </returns>
        public GameObject Spawn(string pool, bool active = true)
        {
            if (!_objPools.ContainsKey(pool))
            {
                throw new Exception($"Object pool doesn't contains the '{pool}'");
            }

            if (_objPools[pool].Count == 0)
            {
                // Creates a new gameobject in runtime expands the pool.
                var prefab = _pools.Find(x => x.key == pool).gameObject;
                GameObject go = Instantiate(prefab, transform, true);
                go.SetActive(false);

                _objPools[pool].Enqueue(go);
            }

            var item = _objPools[pool].Dequeue();
            item.SetActive(active);

            return item;
        }

        /// <summary>
        /// Puts back object to pool.
        /// </summary>
        /// <param name="pool"> Pool name </param>
        /// <param name="item"> Pooled object </param>
        /// <returns> True if it success. </returns>
        public bool DeSpawn(string pool, GameObject item)
        {
            bool result = _objPools.ContainsKey(pool);
            if (result)
            {
                item.SetActive(false);
                item.transform.parent = transform;

                _objPools[pool].Enqueue(item);
            }

            return result;
        }

        /// <summary>
        /// Initialize Objects Pools with the Default Size.
        /// </summary>
        private IEnumerator InitializePools()
        {
            float totalObjectsCount = 0f;
            int createdCount = 0;

            foreach (var pool in _pools)
            {
                totalObjectsCount += pool.count;
            }

            foreach (var pool in _pools)
            {
                // Throw an exception if pool doesn't have a valid prefab.
                if (pool.gameObject == null)
                {
                    throw new Exception("Pool doesn't have a valid prefab object");
                }

                _objPools[pool.key] = new Queue<GameObject>();
                var objPool = _objPools[pool.key];

                for (int i = 0; i < pool.count; ++i)
                {
                    GameObject item = Instantiate(pool.gameObject, transform, true);
                    item.SetActive(false);

                    objPool.Enqueue(item);

                    createdCount++;
                    if (i % 10 == 0)
                    {
                        // Calculate the Progress so UI can give feedback to player.
                        Progress = createdCount / totalObjectsCount;

                        // Let the main thread update after 10 objects initialized.
                        yield return null;
                    }
                }

                Progress = createdCount / totalObjectsCount;
            }
        }

        #endregion region
    }
}