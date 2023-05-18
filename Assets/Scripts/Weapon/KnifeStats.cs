namespace Weapon
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Knife", menuName = "Knife")]
    public class KnifeStats : ScriptableObject
    {
        public float Damage = 25f;
        public float AttackSpeed = 1f;
        public int Range = 5;
    }
}