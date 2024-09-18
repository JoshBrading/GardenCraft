
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
   

    // Method to switch scenes
    public void DeathToGame()
    {
        // Load the scene with the specified index
        Application.Quit();

        Debug.Log("Game off");
    }
}