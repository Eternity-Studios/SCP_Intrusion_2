using UnityEngine;
using Utilities.Gameplay;

namespace Entities
{
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
    public class EntityStats : ScriptableObject
    {
        public int Health;

        public DestroyAfter[] DeathObjects;

        public Factions Faction;
    }

    public enum Factions
    {
        Players,
        Enemy
    }
}
