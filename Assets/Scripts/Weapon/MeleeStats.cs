namespace Weapon
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Melee", menuName = "Melee")]
    public class MeleeStats : ScriptableObject
    {
        public float Damage = 25f;
        public float AttackSpeed = 1f;
        public int Range = 5;
    }
}