using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    using System.Collections.Generic;
    using EntitySystem;

    public class AIBase : NetworkBehaviour
    {
        public bool Paused = false;
        public AIBehavior CurrentBehavior;
        [HideInInspector]
        public NavMeshAgent agent;

        Entity owner;
        private Entity _currentTarget;
        public Entity CurrentTarget
        {
            get => _currentTarget = _currentTarget != null ? _currentTarget : targets.Count > 0 ? _currentTarget = targets[0] : null;
            set
            {
                if (value == null)
                {
                    targets.Remove(_currentTarget);
                    _currentTarget = null;
                }
                if (!targets.Contains(value))
                    targets.Add(value);
                _currentTarget = value;
            }
        }
        [HideInInspector]
        public List<Entity> targets = new();

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            owner = GetComponent<Entity>();
            CurrentBehavior = CurrentBehavior != null ? CurrentBehavior : gameObject.AddComponent<IdleBehavior>();
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