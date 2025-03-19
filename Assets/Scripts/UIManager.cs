using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public TMP_InputField nameInput;
    [SerializeField] public Button menuButton;
    [SerializeField] public Button hostButton;
    [SerializeField] public Button joinButton;
    [SerializeField] public Button startButton;
    [SerializeField] public Button enterButton;
    [SerializeField] private TMP_InputField ipInput; // Input for IP address
    [SerializeField] private TMP_Text HostIpInfo; // Input for IP address
    [SerializeField] private TMP_Text NotificationText;
    void Start()
    {
        menuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Menu"); // Load scene locally

        });
        startButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost) // Check if the player is the host
            {
                NetworkManager.Singleton.SceneManager.LoadScene("OverworldScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) // If NOT in a multiplayer session (single-player)
            {
                SceneManager.LoadScene("OverworldScene"); // Load scene locally
            }
            else
            {
                NotificationText.SetText("Only the host can start the game!");
            }
        });
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            HostIpInfo.SetText("Host started on IP: " + NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Address);
        });

        joinButton.onClick.AddListener(() =>
        {
            GameObject input=ipInput.gameObject;
            input.SetActive(!input.activeSelf);
        });
        enterButton.onClick.AddListener(() =>{
            string ipAddress = ipInput.text; // Get IP from input field
            if (string.IsNullOrEmpty(ipAddress)) ipAddress = "127.0.0.1"; // Default to localhost if empty
            
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            transport.ConnectionData.Address = ipAddress; // Set the IP before joining
            
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client attempting to join host");
            NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    NotificationText.SetText("Connected to host!");
                }
            };
        });
    }
}
