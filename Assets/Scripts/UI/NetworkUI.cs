using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField ipInput;

        [SerializeField]
        Button host;
        [SerializeField]
        Button client;

        [SerializeField]
        int joinGameIndex;
        [SerializeField]
        int mainMenuIndex;

        TabsUI tabs;

        UnityTransport ut;

        private void Awake()
        {
            tabs = GetComponent<TabsUI>();
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;

            ut = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

            ipInput.SetTextWithoutNotify(ut.ConnectionData.Address);

            host.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
            client.onClick.AddListener(() =>
            {
                ut.ConnectionData.Address = ipInput.text;
                NetworkManager.Singleton.StartClient();
                tabs.ActivateTab(joinGameIndex);
            });
        }

        public void OnServerStarted()
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void Shutdown()
        {
            NetworkManager.Singleton.Shutdown();
            tabs.ActivateTab(mainMenuIndex);
        }
    }
}
