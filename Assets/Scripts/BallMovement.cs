using UnityEngine;
using Unity.Netcode;

public class BallMovement : NetworkBehaviour
{
    [SerializeField] private float initialSpeed = 10f; // beginning speed of the ball
    [SerializeField] private float speedIncrease = 0.25f; // increases speed after each hit
    [SerializeField] private AudioSource padHitSound; // Sound played when the ball hits a character
    [SerializeField] private AudioSource goa; // Sound played when the ball scores a goal
    [SerializeField] private AudioSource shoot; // Sound played when the ball is shot

    private Rigidbody2D rb; // Rigidbody component of the ball
    private int hitCounter; // Counter to track the number of hits
    private bool clientJoined = false; // to see  whether a client has joined the game
    
    // Method to make the ball bounce in a specific direction
    public void Bounce(Vector3 direction)
    {
        rb.velocity = direction * (initialSpeed + (speedIncrease * hitCounter));
    }

    // Method to increase the ball's speed
    public void IncreaseBallSpeed(float amount)
    {
        if (IsServer) // Only executed on the server
        {
            initialSpeed += amount;
        }
    } 

    
    void Start()
    {
        // Subscribe to network events
        rb = GetComponent<Rigidbody2D>();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    // Method called when a client connects
    private void OnClientConnected(ulong clientId)
    {
        // If this is the server (host) and the ball hasn't started yet
        if (IsServer)
        {
            // Start the ball movement after a delay
            Invoke("StartBall", 4f);
        }
    }

    // FixedUpdate is called at fixed intervals
    private void FixedUpdate()
    {
        // If the client has joined, adjust the ball speed
        if (clientJoined)
        {
            float currentSpeed = initialSpeed + (speedIncrease * hitCounter);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentSpeed);
        }
    }

    // Method to start the ball movement
    private void StartBall()
    {
        // Randomize starting direction, so we odnt know who the ball will go first to
        float direction = Random.Range(0, 2) == 0 ? -1f : 1f;
        rb.velocity = new Vector3(direction, 0) * initialSpeed;
        
        // Play sound effect
        shootSoundClientRpc();
        
        // Indicate that the client has joined
        clientJoined = true;
    }

    // Call to reset the ball for the client
    [ServerRpc]
    public void ResetBallServerRpc()
    {
        ResetBallClientRpc();
    }

    // Call to reset the ball for the client
    [ClientRpc]
    private void ResetBallClientRpc()
    {
        ResetBall();
    }

    // Call to play shoot sound effect on client
    [ClientRpc]
    public void shootSoundClientRpc()
    {
        shoot.Play(); // Play the shoot sound effect
    }

    // call to play goal sound effect on client
    [ClientRpc]
    public void goalSoundClientRpc()
    {
        goa.Play(); // Play the goal sound effect
    }

    // Procedure call to play paddle hit sound effect on client
    [ClientRpc]
    public void padSoundClientRpc()
    {
        padHitSound.Play(); // Play the paddle hit sound effect
    }

    // Method to reset the ball position and properties after someone scores
    private void ResetBall()
    {
        // Reset ball properties
        rb.velocity = Vector2.zero;
        transform.position = new Vector3(0f, 2.13f, 0f); // Reset position
        hitCounter = 0; // Reset hit counter
        initialSpeed = 10; // Reset initial speed
        gameObject.SetActive(true); // Reactivate the ball in case it was deactivated
        
        // Start the ball movement again when the ball shoots 
        Invoke("StartBall", 2f);
    }

    // Method to handle ball bouncing off player
    public void PlayerBounce(Transform playerTransform)
    {
        // Increase hit counter and adjust ball speed
        hitCounter++;
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        rb.velocity = direction * (initialSpeed + (speedIncrease * hitCounter));
        
        // Play sound effect
        padSoundClientRpc();
    }

    // Method called when the ball hits the object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the ball hits with a player, bounce it
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerBounce(collision.transform);
        }
    }

    // Method called when the ball enters a trigger collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the ball hits the goal, update score and reset the ball
        if (!IsServer) return; // to make sure this is executed only on the server
        if (collision.gameObject.CompareTag("Goal"))
        {
            goalSoundClientRpc(); // Play goal sound effect on client
            bool isPlayerOneScored = transform.position.x < 0; // Check which player scored
            ScoreManager.Instance.UpdateScore(isPlayerOneScored); // Update score in ScoreManager
            ResetBallServerRpc(); // Reset the ball on all client
        }
    }
}
