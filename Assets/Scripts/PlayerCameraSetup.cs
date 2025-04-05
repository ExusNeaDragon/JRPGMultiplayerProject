using UnityEngine;
using Unity.Netcode;

public class PlayerCameraSetup : NetworkBehaviour
{
    void Start()
    {
        Camera cam = GetComponentInChildren<Camera>();
        AudioListener listener = cam.GetComponent<AudioListener>();

        // If networking is active, only the owner gets the camera and audio
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            if (IsOwner)
            {
                var follow = cam.GetComponent<CameraFollow>();
                if (follow != null) follow.SetTarget(transform);
                if (listener != null) listener.enabled = true;
            }
            else
            {
                cam.enabled = false;
                if (listener != null) listener.enabled = false;
            }
        }
        else
        {
            // No network manager = assume single player
            var follow = cam.GetComponent<CameraFollow>();
            if (follow != null) follow.SetTarget(transform);
            if (listener != null) listener.enabled = true;
        }
    }
}
