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
        [SerializeField]
        private float StopDistance = -1f;
        protected virtual void FixedUpdate()
        {
            if (!IsServer)
                return;

            var target = AI.CurrentTarget;
            if (target != null) // nvm
            {
                var position = target.transform.position;
                var distance = Vector3.Distance(AI.transform.position, position);
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (StopDistance != -1 && distance <= StopDistance)
                {
                    AI.agent.isStopped = true;
                }
                else
                {
                    AI.agent.isStopped = false;
                    AI.agent.SetDestination(position);
                }
                if (_range < 0f || (_range > 0f && distance <= _range))
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
