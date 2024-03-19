using UnityEngine;
using Unity.Netcode;

public class BallInstantiator : NetworkBehaviour
{
    // Reference to the ball prefab to respawn.
    public GameObject ballPrefab;

    // to make sure the ball is respawns only once.
    private bool ballSpawned = false;

    void Start()
    {
        // Subscribe to the event that is called when a client connects.
        // This makes sure the ball is spawned when the server is ready and clients are connected.
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Check if the current instance is the server and if the ball is there yet.
        if (IsServer && !ballSpawned)
        {
            // putting the ball prefab here at this spot
            // The position is set to (0, 2.13, 0) to put the ball in the starting spot where the lines are, the middle of the map
            GameObject ball = Instantiate(ballPrefab, new Vector3(0f, 2.13f, 0f), Quaternion.identity);

            // getting the NetworkObject component of the ball.
            NetworkObject ballNetworkObject = ball.GetComponent<NetworkObject>();
            if (ballNetworkObject != null)
            {
                // Spawn the ball across the network, making it exist and be synced for client
                ballNetworkObject.Spawn();
            }

            // Mark the ball as spawned so it doesnt spawn again
            ballSpawned = true;
        }
    }
}
