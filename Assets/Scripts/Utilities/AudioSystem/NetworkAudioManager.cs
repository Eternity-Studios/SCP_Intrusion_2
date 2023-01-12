using Unity.Netcode;
using UnityEngine;

namespace Utilities.Audio
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkAudioManager : NetworkBehaviour
    {


        [ClientRpc]
        public void PlaySoundClientRpc()
        {

        }
    }
}
