namespace EntitySystem
{
    using UnityEngine;
    using Utilities.Gameplay;

    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
    public class EntityStats : ScriptableObject
    {
        public float Health;

        public DestroyAfter[] DeathObjects;

        public Factions Faction;
    }

    public enum Factions
    {
        Players,
        Enemy
    }
}
