using System;
using Unity.Netcode;
using UnityEngine;

public class QuickJoinPlayerEditor : MonoBehaviour
{
    private String joinCode;
    void Start()
    {
        if (Application.isEditor) NetworkManager.Singleton.StartHost();
        if (!Application.isEditor) NetworkManager.Singleton.StartClient();
    }

    void OnGUI()
    {
        if (!Application.isEditor) return;
        if (GUI.Button(new Rect(10, 10, 150, 50), "Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUI.Button(new Rect(10, 60, 150, 50), "Client Local"))
        {
            NetworkManager.Singleton.StartClient();
        }

        if (GUI.Button(new Rect(10, 200, 150, 50), "Client w/ Join Code"))
        {
            NetworkManager.Singleton.StartClient();
        }
    }


}
