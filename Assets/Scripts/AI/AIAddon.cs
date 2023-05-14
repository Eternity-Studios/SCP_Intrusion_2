using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(AIBase))]
    public abstract class AIAddon : MonoBehaviour
    {
        public AIBase ai;

        private void Awake()
        {
            ai = GetComponent<AIBase>();
        }
    }
}
