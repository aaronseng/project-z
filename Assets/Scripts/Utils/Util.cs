using System.Collections.Generic;
using UnityEngine;

namespace ProjectZ.Helpers
{
    public static class Util
    {
        /// <summary>
        /// Returns a random number between [0..max) excluded given list
        /// </summary>
        /// <param name="max">Maximum number</param>
        /// <param name="exclude">Excluded numbers</param>
        /// <returns>Random number</returns>
        public static int RandomExceptList(int max, List<int> exclude)
        {
            exclude.Sort();
            int result = Random.Range(0, max - exclude.Count);

            foreach (int exc in exclude)
            {
                if (result < exc)
                {
                    return result;
                }
                result++;
            }
            return result;
        } 
    }
}