using Unity.Netcode;
using UnityEngine;

namespace Interactables
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public class Interact : NetworkBehaviour
    {

    }
}
