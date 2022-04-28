using System.Threading.Tasks;
using UnityEngine;

public static class MonoBehaviourExtension
{
    /// <summary>
    /// HACKAROUND: WebGL "Task.Delay" actually doesn't work in Unity.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="delay"> Delay in seconds </param>
    public static async Task Delay(this MonoBehaviour script, float delay)
    {
        float start = Time.realtimeSinceStartup;
        while ((Time.realtimeSinceStartup - start) < delay)
        {
            await Task.Yield();
        }
    }
}