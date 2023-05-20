namespace AI
{
    using EntitySystem;
    using Unity.Netcode;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIBase))]
    public abstract class AIBehavior : NetworkBehaviour
    {
        public AIBase AI { get; private set; }

        protected virtual float GetRange => -1f;
        private float _range;
        protected virtual void FixedUpdate()
        {
            if (!IsServer)
                return;

            var target = AI.CurrentTarget;
            if (target != null) // nvm
            {
                AI.agent.SetDestination(target.transform.position);
                if (_range > 0f && Vector3.Distance(AI.transform.position, target.transform.position) <= _range)
                {
                    OnInsideRange(target);
                }
            }
        }

        protected virtual void OnInsideRange(Entity target)
        {
        }

        public virtual void OnTargetAdded(Entity target)
        {
        }

        protected virtual void Awake()
        {
            AI = GetComponent<AIBase>();
            _range = GetRange;
        }
    }
}
