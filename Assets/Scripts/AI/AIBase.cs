using Entities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIBase : NetworkBehaviour
    {
        NavMeshAgent agent;

        Entity owner;
        Entity target;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            owner = GetComponent<Entity>();
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            if (target != null)
                agent.SetDestination(target.transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            if (target == null && other.CompareTag("Targetable") && other.TryGetComponent(out Entity ent) && ent.entity.Faction != owner.entity.Faction)
            {
                target = ent;
            }
        }
    }
}