using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode; // Optional - only used if NetworkManager exists

public class UIManager : MonoBehaviour
{
    [SerializeField] public Button menuButton;
    [SerializeField] public Button hostButton;
    [SerializeField] public Button joinButton;
    [SerializeField] public Button startButton;
    [SerializeField] public Button enterButton;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_Text HostIpInfo;
    [SerializeField] private TMP_Text NotificationText;
    [SerializeField] public TMP_InputField nameInput;

    private bool hasNetworkManager;

    void Start()
    {
        // Check if NetworkManager exists
        hasNetworkManager = (NetworkManager.Singleton != null);

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() =>
            {
                if (SceneManager.GetActiveScene().name != "Menu")
                {
                    Destroy(GameObject.Find("NetworkManager"));
                    SceneManager.LoadScene("Menu");
                }
            });
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                if (!hasNetworkManager) // No NetworkManager → Single Player Mode
                {
                    Debug.Log("Starting Single Player Mode");
                    SceneManager.LoadScene("Level 1");
                }
                else if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
                {
                    Debug.Log("Starting Single Player Mode");
                    Destroy(GameObject.Find("NetworkManager"));
                    SceneManager.LoadScene("Level 1"); // Single Player
                }
                else if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("Starting Multi Player Mode");
                    NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
                }
                else
                {
                    NotificationText.SetText("Only the host can start the game!");
                }
            });
        }

        if (hostButton != null && joinButton != null && enterButton != null && ipInput != null)
        {
            hostButton.onClick.AddListener(() =>
            {
                if (!hasNetworkManager)
                {
                    Debug.LogError("NetworkManager is missing! Cannot start host.");
                    NotificationText.SetText("Error: No network manager found.");
                    return;
                }

                NetworkManager.Singleton.StartHost();
                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

                if (transport != null)
                {
                    HostIpInfo.SetText("Host started on IP: " + transport.ConnectionData.Address);
                }
                else
                {
                    HostIpInfo.SetText("Host started, but no transport found.");
                }
            });

            joinButton.onClick.AddListener(() =>
            {
                ipInput.gameObject.SetActive(!ipInput.gameObject.activeSelf);
            });

            enterButton.onClick.AddListener(() =>
            {
                if (!hasNetworkManager)
                {
                    Debug.LogError("NetworkManager is missing! Cannot join.");
                    NotificationText.SetText("Error: No network manager found.");
                    return;
                }

                string ipAddress = ipInput.text.Trim();
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = "127.0.0.1";

                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                if (transport != null)
                {
                    transport.ConnectionData.Address = ipAddress;
                    NetworkManager.Singleton.StartClient();

                    Debug.Log("Client attempting to join host at " + ipAddress);
                    NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
                    {
                        if (NetworkManager.Singleton.LocalClientId == clientId)
                        {
                            NotificationText.SetText("Connected to host!");
                        }
                    };
                }
                else
                {
                    Debug.LogError("No transport found! Cannot connect.");
                    NotificationText.SetText("Error: No transport found.");
                }
            });
        }
    }
}
