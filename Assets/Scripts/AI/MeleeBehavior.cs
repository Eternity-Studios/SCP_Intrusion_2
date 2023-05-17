namespace AI
{
    using Entities;

    public class MeleeBehavior : AIBehavior
    {
        public override void OnInsideRange(Entity target)
        {
        }

        public MeleeBehavior(AIBase ai) : base(ai)
        {
        }
    }
}
