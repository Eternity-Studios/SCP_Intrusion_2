namespace Utilities.Networking 
{
    using global::Player.Interact;
    using global::Player.Management;
    using global::Player.Movement;
    using Guns;
    using UnityEngine;
    using Utilities.Player;

    public class ReferenceHub : MonoBehaviour
    {
        public PlayerController controller;

        public Gun weapon;

        public PlayerMovement movement;

        public PlayerLook look;

        public PlayerLogic logic;

        public PlayerInteract interact;
    }
}
