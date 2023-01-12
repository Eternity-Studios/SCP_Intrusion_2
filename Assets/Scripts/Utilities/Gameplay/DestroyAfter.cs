using UnityEngine;

namespace Utilities.Gameplay
{
    public class DestroyAfter : MonoBehaviour
    {
        public float Timer;

        private void Start()
        {
            Destroy(gameObject, Timer);
        }
    }
}
