using Guns;
using Player.Interact;
using Player.Management;
using Player.Movement;
using UnityEngine;

namespace Player 
{
    public class ReferenceHub : MonoBehaviour
    {
        public Gun weapon;

        public PlayerMovement movement;

        public PlayerLook look;

        public PlayerLogic logic;

        public PlayerInteract interact;
    }
}
