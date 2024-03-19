using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Block : NetworkBehaviour
{

    private void Start()
    {
        // Call the DestroyPowerUp method after 5 seconds.
        Invoke(nameof(DestroyPowerUp), 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {       //if the ball hits the block
        if (other.gameObject.CompareTag("Ball"))
        {
            

            // Access the Rigidbody component attached to the ball
            Rigidbody2D ballRigidbody = other.gameObject.GetComponent<Rigidbody2D>();

            // Check if the Rigidbody component was found
            if (ballRigidbody != null)
            {
                // throw the velocity of the ball to make it bounce back to the player
                ballRigidbody.velocity = new Vector2(-ballRigidbody.velocity.x, ballRigidbody.velocity.y);
                
            }

            // Cancel the pending destruction and kill the block
            CancelInvoke(nameof(DestroyPowerUp));
            Destroy(gameObject);
        }
    }

    // Method to destroy the block if not hit by the ball
    private void DestroyPowerUp()
    {
        Debug.Log("Block destroyed due to timeout.");
        Destroy(gameObject);
    }
}
