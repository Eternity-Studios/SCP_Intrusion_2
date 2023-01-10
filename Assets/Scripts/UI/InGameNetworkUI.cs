using Player.Movement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
                PlayerLook.OwnedInstance.SetState(false);
            }
            else
            {
                tabs.ActivateTab(gameUIIndex);
                PlayerLook.OwnedInstance.SetState(true);
            }
        }

        public void Resume()
        {
            tabs.ActivateTab(gameUIIndex);
            PlayerLook.OwnedInstance.SetState(true);
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
