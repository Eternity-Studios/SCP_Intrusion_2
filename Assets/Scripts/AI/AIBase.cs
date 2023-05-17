using Entities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    using System.Collections.Generic;

    public class AIBase : NetworkBehaviour
    {
        public bool IsStopped => agent.isStopped;
        public AIBehavior CurrentBehavior;
        [HideInInspector]
        public NavMeshAgent agent;

        Entity owner;
        private Entity _currentTarget;
        public Entity CurrentTarget
        {
            get => _currentTarget ??= targets.Count > 0 ? _currentTarget = targets[0] : null;
            set
            {
                if (!targets.Contains(value))
                    targets.Add(value);
                _currentTarget = value;
            }
        }
        [HideInInspector]
        public List<Entity> targets = new List<Entity>();

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            owner = GetComponent<Entity>();
            CurrentBehavior ??= gameObject.AddComponent<IdleBehavior>();
        }

        private void FixedUpdate()
        {
            if (!IsServer)
                return;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer)
                return;

            if (other.CompareTag("Targetable") && other.TryGetComponent(out Entity ent) && ent.entity.Faction != owner.entity.Faction)
            {
                targets.Add(ent);
                CurrentBehavior.OnTargetAdded(ent);
            }
        }
    }
}