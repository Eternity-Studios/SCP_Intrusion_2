using Player.Management;
using Player.Movement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utilities.Networking;

namespace UI
{
    [RequireComponent(typeof(TabsUI))]
    [DisallowMultipleComponent]
    public class InGameNetworkUI : MonoBehaviour
    {
        [SerializeField]
        int pauseMenuIndex;
        [SerializeField]
        int gameUIIndex;

        TabsUI tabs;

        Game inputActions;

        private void Awake()
        {
            tabs = GetComponent<TabsUI>();

            inputActions = new Game();

            inputActions.Player.Pause.performed += PauseMenu;

            inputActions.Enable();
        }

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId != NetworkManager.Singleton.LocalClientId && clientId != NetworkManager.ServerClientId)
                return;

            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            SceneManager.LoadScene("Menu");
        }

        private void OnDestroy()
        {
            inputActions.Player.Pause.performed -= PauseMenu;
        }

        public void PauseMenu(InputAction.CallbackContext callbackContext)
        {
            if (tabs.CurrentIndex != pauseMenuIndex)
            {
                tabs.ActivateTab(pauseMenuIndex);
                PlayerLogic.OwnedInstance.referenceHub.look.SetState(false);
            }
            else
            {
                tabs.ActivateTab(gameUIIndex);
                PlayerLogic.OwnedInstance.referenceHub.look.SetState(true);
            }
        }

        public void Resume()
        {
            tabs.ActivateTab(gameUIIndex);
            PlayerLogic.OwnedInstance.referenceHub.look.SetState(true);
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("Menu");
        }
    }
}
