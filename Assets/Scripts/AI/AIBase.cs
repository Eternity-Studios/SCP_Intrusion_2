using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIBase : MonoBehaviour
    {
        NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
}