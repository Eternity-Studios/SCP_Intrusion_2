using Entities;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIBase : MonoBehaviour
    {
        NavMeshAgent agent;

        Entity target;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void FixedUpdate()
        {
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }
}