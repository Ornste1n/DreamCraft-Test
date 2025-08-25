using UnityEngine;

namespace Utilities
{
    public class AutoKillMono : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}
