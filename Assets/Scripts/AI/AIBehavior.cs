namespace AI
{
    using Entities;
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIBase))]
    public abstract class AIBehavior : NetworkBehaviour
    {
        public AIBase AI { get; private set; }
        [SerializeField]
        protected float _range = -1f;

        protected virtual void FixedUpdate()
        {
            if (!IsServer)
                return;
            var target = AI.CurrentTarget;
            if (target is not null) // is null is up to 400x faster than == null cause unity; in this case it doesn't matter since we return null ourselves if not found
            {
                AI.agent.SetDestination(target.transform.position);
                if (_range > 0f && Vector3.Distance(AI.transform.position, target.transform.position) <= _range)
                {
                    OnInsideRange(target);
                }
            }
        }

        public virtual void OnInsideRange(Entity target)
        {
        }

        public virtual void OnTargetAdded(Entity target)
        {
            
        }

        private void Awake()
        {
            AI = GetComponent<AIBase>();
        }

        public AIBehavior(AIBase ai)
        {
            AI = ai;
        }
    }
}
