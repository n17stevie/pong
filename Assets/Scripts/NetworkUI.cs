using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn; // Button to start a host
    [SerializeField] private Button clientBtn; // Button to start a client

   
    private void Awake()
    {
        // Add listener to the host button
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost(); // Start a host when the host button is clicked
        });

        // Add listener to the client button
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient(); // Start a client when the client button is clicked
        });
    }
}
