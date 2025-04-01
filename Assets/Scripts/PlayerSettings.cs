using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField]private TMP_Text playerName;
    NetworkVariable<NetworkString> networkPlayerName= new NetworkVariable<NetworkString>("Unknown", NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn(){
        if(IsOwner){
            networkPlayerName.Value=GameObject.Find("UIManager").GetComponent<UIManager>().nameInput.text;
        }
        playerName.text = networkPlayerName.Value.ToString();
        networkPlayerName.OnValueChanged+=NetworkPlayerName_OnValueChanged;
    }
    void NetworkPlayerName_OnValueChanged(NetworkString previousValue, NetworkString newValue)
    {
        playerName.text=newValue;
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