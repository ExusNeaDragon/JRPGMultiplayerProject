using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using MyGameNamespace;
using System.Net;
using System.Net.Sockets;
using System.Collections;

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
    private UdpClient udpClient;
    private IPEndPoint endPoint;

    void Start()
    {
        // Default to Single Player mode
        GameState.IsSinglePlayer = true;

        // Check if NetworkManager exists
        hasNetworkManager = (NetworkManager.Singleton != null);

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() =>
            {
                if (SceneManager.GetActiveScene().name != "Menu")
                {
                    // Load the Menu Scene without destroying the NetworkManager
                    SceneManager.LoadScene("Menu");
                }
            });
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                if (!hasNetworkManager || (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer))
                {
                    // Single-player mode
                    Debug.Log("Starting Single Player Mode");
                    GameState.IsSinglePlayer = true;  // Set single-player mode
                    SceneManager.LoadScene("Level 1");
                }
                else if (NetworkManager.Singleton.IsHost)
                {
                    // Multiplayer mode as host
                    Debug.Log("Starting Multi Player Mode");
                    GameState.IsSinglePlayer = false;  // Set multiplayer mode
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

                string localIP = GetLocalWiFiIPAddress();

                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                if (transport != null)
                {
                    transport.ConnectionData.Address = localIP;
                }

                NetworkManager.Singleton.StartHost();

                if (transport != null)
                {
                    HostIpInfo.SetText("Host started on IP: " + transport.ConnectionData.Address);
                }
                else
                {
                    HostIpInfo.SetText("Host started, but no transport found.");
                }

                GameState.IsSinglePlayer = false;
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

                string enteredIp = ipInput.text;

                // If the input is empty, start the host discovery
                if (string.IsNullOrEmpty(enteredIp))
                {
                    NotificationText.SetText("Discovering host...");
                    StartCoroutine(DiscoverHostAndConnect());
                }
                else
                {
                    // Ensure the input is a valid IP format
                    if (!IsValidIP(enteredIp))
                    {
                        NotificationText.SetText("Invalid IP address.");
                        return;
                    }

                    ConnectToHost(enteredIp);
                }
            });

            // Helper method to validate IP format



                    }
    }
    private bool IsValidIP(string ip)
    {
        string[] parts = ip.Split('.');
        if (parts.Length == 4)
        {
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int byteValue) || byteValue < 0 || byteValue > 255)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    private IEnumerator DiscoverHostAndConnect()
    {
        // Set up UDP client to broadcast to the network
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Broadcast, 7777); // Use a broadcast port

        // Send broadcast message
        byte[] data = System.Text.Encoding.UTF8.GetBytes("DISCOVER_SERVER");
        udpClient.Send(data, data.Length, endPoint);

        // Wait for a response (10-second timeout)
        float timeout = 10f;
        string hostIp = string.Empty;

        while (timeout > 0f)
        {
            if (udpClient.Available > 0)
            {
                byte[] response = udpClient.Receive(ref endPoint);
                string message = System.Text.Encoding.UTF8.GetString(response);

                if (message.StartsWith("SERVER_RESPONSE"))
                {
                    hostIp = message.Split(':')[1];
                    break;
                }
            }

            timeout -= Time.deltaTime;
            yield return null;
        }

        if (string.IsNullOrEmpty(hostIp))
        {
            NotificationText.SetText("Host not found. Please check the network.");
        }
        else
        {
            // Connect to the discovered host
            NotificationText.SetText("Found Host: " + hostIp);
            ConnectToHost(hostIp);
        }

        udpClient.Close();
    }

    private void ConnectToHost(string hostIp)
    {
        // If no IP is provided, default to localhost
        string ipAddress = string.IsNullOrEmpty(hostIp) ? "127.0.0.1" : hostIp;

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = ipAddress;

            NotificationText.SetText("Connecting to host...");
            StartCoroutine(ConnectionTimeoutCheck(10f)); // Timeout after 10 seconds

            NetworkManager.Singleton.StartClient();

            Debug.Log("Client attempting to join host at " + ipAddress);

            // Callback to confirm connection
            NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
            {
                if (NetworkManager.Singleton.LocalClientId == clientId)
                {
                    StopAllCoroutines(); // Stop the timeout check if connected
                    NotificationText.SetText("Connected to host!");
                }
            };

            GameState.IsSinglePlayer = false;
        }
        else
        {
            Debug.LogError("No transport found! Cannot connect.");
            NotificationText.SetText("Error: No transport found.");
        }
    }

    private IEnumerator ConnectionTimeoutCheck(float timeoutDuration)
    {
        float elapsed = 0f;

        while (elapsed < timeoutDuration)
        {
            if (NetworkManager.Singleton.IsConnectedClient)
            {
                yield break; // Already connected
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        NotificationText.SetText("Connection timed out. Check IP and try again.");
        Debug.LogWarning("Connection attempt timed out.");
    }

    private string GetLocalWiFiIPAddress()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();  // Return first IPv4 found
            }
        }
        return "127.0.0.1"; // Default fallback
    }
}
