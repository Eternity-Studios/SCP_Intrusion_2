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
        GameObject mainMenu;

        [SerializeField]
        TMP_InputField ipInput;

        [SerializeField]
        Button host;
        [SerializeField]
        Button client;

        UnityTransport ut;

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
            });
        }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }

        public void OnServerStarted()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
