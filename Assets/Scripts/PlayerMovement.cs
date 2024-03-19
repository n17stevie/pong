using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    // speed of the player
    [SerializeField] private float movementSpeed = 5f;
    
    // Audio source for player movement sound
    [SerializeField] private AudioSource movSound; 

    // Rigidbody component of the player
    private Rigidbody2D rb;

    // player movement controls
    private KeyCode upKey;
    private KeyCode downKey;

    // Direction of player movement
    private Vector2 playerMove;

    // Reference to the SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // to check if the object is the local player
    private bool isLocalPlayer = false;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Determine if this object is the local player
        if (IsOwner)
        {
            isLocalPlayer = true;

            // Set position and controls based on local player
            if (NetworkManager.Singleton.IsServer)
            {
                transform.position = new Vector3(-7.5f, 0f, 0f); // Host position
                upKey = KeyCode.UpArrow;
                downKey = KeyCode.DownArrow;
                spriteRenderer.flipX = false;
                //making sure it is not flipped
            }
            else
            {
                transform.position = new Vector3(7.5f, 0f, 0f); // Client position
                upKey = KeyCode.UpArrow;
                downKey = KeyCode.DownArrow;
                spriteRenderer.flipX = true; // to make the client face player
                spriteRenderer.color = Color.red; // Set color to red for the client
            }
        }
        else // This is not the local player
        {
            // Flip the sprite and set color for the host
            if (NetworkManager.Singleton.IsServer)
            {
                spriteRenderer.flipX = true;
                spriteRenderer.color = Color.red; // Set color to red for the host
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // Check player controls
            Player1Control();
        }
    }

    // Player 1 control method
    private void Player1Control()
    {
        // Only respond to arrow up and down keys
        playerMove = Vector2.zero;
        if (Input.GetKey(upKey))
        {
            playerMove = Vector2.up;
            movSound.Play(); // Play movement sound when moving up
        }
        else if (Input.GetKey(downKey))
        {
            playerMove = Vector2.down;
            movSound.Play(); // Play movement sound when moving down
        }
    }
    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // Apply movement based on player input
            rb.velocity = playerMove * movementSpeed;
        }
    }
}
