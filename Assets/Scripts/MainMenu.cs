using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
[SerializeField] private AudioSource click;

    // starts the game after we press start
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        click.Play();
    }
}
