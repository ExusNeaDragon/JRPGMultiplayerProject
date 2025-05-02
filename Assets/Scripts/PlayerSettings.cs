using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerTag;
    private static string uniquePlayerTag = "Unknown";

    // Networked variable for player name, synchronized across the network
    NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes("Unknown"), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {

        // Update the player name text when it changes on the network
        networkPlayerName.OnValueChanged += NetworkPlayerName_OnValueChanged;

        if (IsOwner)
        {
            // Only set player name from UIManager in the first scene
            if (string.IsNullOrEmpty(uniquePlayerTag))
            {
                var uiManager = GameObject.Find("UIManager");
                if (uiManager != null)
                {
                    uniquePlayerTag = uiManager.GetComponent<UIManager>().nameInput.text;
                }
                else
                {
                    uniquePlayerTag = $"Player{OwnerClientId}";  // Default if UIManager is not found
                }

                // Set the player’s networked name
                networkPlayerName.Value = new FixedString32Bytes(uniquePlayerTag);
            }

            // Rename the player object to "Player 1", "Player 2", etc.
            gameObject.name = $"Player {OwnerClientId}";
        }

        // Update the displayed player name
        playerTag.text = networkPlayerName.Value.ToString();
    }

    void NetworkPlayerName_OnValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerTag.text = newValue.ToString();  // Update UI text when player name changes
    }

    // Method to get the unique player tag
    public static string GetUniquePlayerTag()
    {
        return uniquePlayerTag;
    }
}

public struct NetworkString :INetworkSerializeByMemcpy{
    private ForceNetworkSerializeByMemcpy<FixedString32Bytes>_info;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
    where T : IReaderWriter
    {
        serializer.SerializeValue(ref _info);
    }
    public override string ToString()
    {
        return _info.Value.ToString();
    }
    public static implicit operator string(NetworkString s)=> s.ToString();
    public static implicit operator NetworkString(string s)=> new NetworkString(){_info=new FixedString32Bytes(s)};
}