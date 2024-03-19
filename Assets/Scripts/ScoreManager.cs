using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; } // Singleton instance of the ScoreManager

    [SerializeField] private Text playerScoreText; // for the player's score
    [SerializeField] private Text player2ScoreText; // for the clients score
    [SerializeField] private Text gameOverText; // displays game over message
    [SerializeField] private Text gameOverText2; // displays game over for 2nd player
    [SerializeField] private Text first2Seven; // "First to Seven" message for the first screen
    [SerializeField] private Text first2Seven2; // v"First to Seven" message for 2nd screen

    public NetworkVariable<int> playerScore = new NetworkVariable<int>(); // NetworkVariable to sync player's score
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(); // NetworkVariable to sync player 2's score

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // to make sure there is only one instance of this GameObject
        }
        else
        {
            Instance = this;
        }
    }

    //to put how long he want to show "first to seven on the screen for both players"
    private IEnumerator ShowFirstToSevenText()
    {
        // Activate the text
        first2Seven.gameObject.SetActive(true);
        first2Seven2.gameObject.SetActive(true);

        // Wait for 7 seconds
        yield return new WaitForSeconds(11f);

        // Deactivate the text
        first2Seven.gameObject.SetActive(false);
        first2Seven2.gameObject.SetActive(false);
    }

   
    private void Start()
    {
        playerScore.OnValueChanged += OnPlayerScoreChanged; // Subscribe to playerScore change event
        player2Score.OnValueChanged += OnPlayer2ScoreChanged; // Subscribe to player2Score change event
        gameOverText.gameObject.SetActive(false); // Deactivate game over to show for player 1
        gameOverText2.gameObject.SetActive(false); // Deactivate game over to show for player 2
        StartCoroutine(ShowFirstToSevenText()); // so we can show first 2 seven rule
    }

    // Method to update score based on which player scored
    public void UpdateScore(bool isPlayerOneScored)
    {
        if (IsServer) // to make sure the update is called on the server
        {
            if (isPlayerOneScored)
            {
                player2Score.Value += 1; // to add a score player 2's score if they scored
                if (player2Score.Value >= 7) // Check if player 2 got 7 points
                {
                    EndGame(); // Call method to end the game
                }
            }
            else
            {
                playerScore.Value += 1; // adds a score to player's 1 score
                if (playerScore.Value >= 7) // Check if player got 7 points
                {
                    EndGame(); // Call method to end the game
                }
            }
        }
    }

    // Event to handle the playerScore change event
    private void OnPlayerScoreChanged(int oldValue, int newValue)
    {
        playerScoreText.text = newValue.ToString(); // Update player 1 score text
    }

    // Event handler for player2Score change event
    private void OnPlayer2ScoreChanged(int oldValue, int newValue)
    {
        player2ScoreText.text = newValue.ToString(); // Update player 2 score text
    }

    // procedure call to show game over text on client
    [ClientRpc]
    public void ShowGameOverTextClientRpc()
    {
        gameOverText.gameObject.SetActive(true); // turn on game over when the game gets to 7
        gameOverText2.gameObject.SetActive(true); // turn on game over when the game gets to 7
        Time.timeScale = 0; // Pause the game when someone scored 7
        AudioListener.volume = 0f; // turns off audio of the game when its over
    }

    // Method to end the game
    private void EndGame()
    {
        // Reset both player scores to 0
        playerScore.Value = 0;
        player2Score.Value = 0;
        ShowGameOverTextClientRpc(); // Show game over text on client
    }

    // OnDestroy is called when the GameObject is killed
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        // Unsubscribe from score change events
        playerScore.OnValueChanged -= OnPlayerScoreChanged;
        player2Score.OnValueChanged -= OnPlayer2ScoreChanged;
    }
}
