using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.UI
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        GameObject lobbyCam;
        [SerializeField]
        GameObject mainMenu;

        [SerializeField]
        TMP_InputField ipInput;

        [SerializeField]
        Button host;
        [SerializeField]
        Button server;
        [SerializeField]
        Button client;

        UnityTransport ut;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;

            ut = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

            ipInput.SetTextWithoutNotify(ut.ConnectionData.Address);

            host.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
            server.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
            client.onClick.AddListener(() =>
            {
                ut.ConnectionData.Address = ipInput.text;
                NetworkManager.Singleton.StartClient();
            });
        }

        public void OnConnect(ulong clientId)
        {
            lobbyCam.SetActive(false);
            mainMenu.SetActive(false);
        }

        public void OnDisconnect(ulong clientId)
        {
            lobbyCam.SetActive(true);
            mainMenu.SetActive(true);
        }
    }
}
