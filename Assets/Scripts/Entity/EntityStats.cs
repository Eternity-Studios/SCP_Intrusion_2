using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
    public class EntityStats : ScriptableObject
    {
        public int Health;

        public GameObject[] DeathObjects;

        public Factions Faction;
    }

    public enum Factions
    {
        Players,
        Enemy
    }
}
