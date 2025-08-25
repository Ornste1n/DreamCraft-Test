using UnityEngine;

namespace Utilities
{
    public static class ApplicationSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Execute()
        {
#if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 60;
#elif UNITY_STANDALONE
            Application.targetFrameRate = 120;
#endif
            QualitySettings.vSyncCount = 0;
        }
    }
}
