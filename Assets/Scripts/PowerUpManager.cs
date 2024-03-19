using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PowerUpManager : NetworkBehaviour
{
    public GameObject powerUpPrefab; // Assigning the power-up prefab
    public GameObject blockPrefab;   // Assigning the block prefab
    public float powerUpSpawnInterval = 10f; // Time between power-up respawns
    public float blockSpawnInterval = 5f;    // Time between block respawns

    // range of where the power up will respawn on the x axis
    private float spawnXMin = -5.84f;
    private float spawnXMax = 5.84f;   

    // range of power up respawns on the y axis
    private float spawnYMin = -4.5f;
    private float spawnYMax = 4.5f;  
    [SerializeField] private AudioSource spedup; // audio sources
    [SerializeField] private AudioSource block;
    void Start()
    {
        // Subscribe to the client connected event
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Start spawning power-ups and blocks only on the server
        if (IsServer)
        {
            InvokeRepeating(nameof(SpawnPowerUp), powerUpSpawnInterval, powerUpSpawnInterval);
            InvokeRepeating(nameof(SpawnBlock), blockSpawnInterval, blockSpawnInterval);
        }
    }

    // This method will be called on the server, and call play() on client
[ClientRpc]
public void PlaySpeedUpSoundClientRpc()
{
    spedup.Play();
}


// This method will be called on the server, and execute Play() on client
[ClientRpc]
public void PlayBlockSoundClientRpc()
{
    block.Play(); 
}


    void SpawnPowerUp()
    {
        // Generate a random position within the spawn area
        float randomX = Random.Range(spawnXMin, spawnXMax);
        float randomY = Random.Range(spawnYMin, spawnYMax);
        Vector2 randomPosition = new Vector2(randomX, randomY);

        // Instantiate the power-up at the random position
        GameObject spawnedPowerUp = Instantiate(powerUpPrefab, randomPosition, Quaternion.identity);

        // Spawn the power-up on the network
        NetworkObject powerUpNetworkObject = spawnedPowerUp.GetComponent<NetworkObject>();
        if (powerUpNetworkObject != null)
        {
            powerUpNetworkObject.Spawn();
        }

        //kill power-up after 5 seconds if not picked up
        Destroy(spawnedPowerUp, 5f);
        PlaySpeedUpSoundClientRpc();
        
    }

    void SpawnBlock()
    {
        // random position within the spawn area
        float randomX = Random.Range(spawnXMin, spawnXMax);
        float randomY = Random.Range(spawnYMin, spawnYMax);
        Vector2 randomPosition = new Vector2(randomX, randomY);

        // Instantiate the block at the random position
        GameObject spawnedBlock = Instantiate(blockPrefab, randomPosition, Quaternion.identity);

        // Spawn the block on the network
        NetworkObject blockNetworkObject = spawnedBlock.GetComponent<NetworkObject>();
        if (blockNetworkObject != null)
        {
            blockNetworkObject.Spawn();
        }

        // destroy the block after 5 seconds if not picked up
        Destroy(spawnedBlock, 5f);
       PlayBlockSoundClientRpc();
    }
}
