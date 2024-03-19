using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class BallSpeedPowerUp : NetworkBehaviour
{
    public float speedIncreaseAmount = 60f; // to speed up the ball speed
    

    // Start is called when the object becomes active in the scene.
    private void Start()
    {
        // this method is called after 5 seconds.
        Invoke(nameof(DestroyPowerUp), 5f);
    

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // dont destroy if the ball hits the power-up.
            CancelInvoke(nameof(DestroyPowerUp));
            
            
            BallMovement ballMovement = other.gameObject.GetComponent<BallMovement>();
            if (ballMovement != null)
            {
                Rigidbody2D ballRigidbody = other.GetComponent<Rigidbody2D>();
                
                // needed to log the speed to see if it worked
                Debug.Log("Speed before: " + ballRigidbody.velocity.magnitude);

                // Increase the ball's speed
                ballMovement.IncreaseBallSpeed(speedIncreaseAmount);
                

                
                // Start coroutine to log speed after a physics update
                StartCoroutine(LogSpeedAfterFixedUpdate(ballRigidbody));

                // Destroy the power-up since it's been hit by the ball.
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("BallMovement script not found on the ball object.");
            }
        }
    }

    // logging the ball's speed
    private IEnumerator LogSpeedAfterFixedUpdate(Rigidbody2D ballRigidbody)
    {
        // Wait until next physics update
        yield return new WaitForFixedUpdate();

        // Log the speed after the increase
        Debug.Log("Speed after: " + ballRigidbody.velocity.magnitude);
    }

    // kills the power-up after a delay
    private void DestroyPowerUp()
    {
        Debug.Log("Power-up destroyed due to timeout.");
        Destroy(gameObject);
    }
}