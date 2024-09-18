
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchSceneButton : MonoBehaviour
{
    // The index of the scene to switch to
    public int sceneIndex;

    // Method to switch scenes
    public void SwitchScene()
    {
        // Load the scene with the specified index
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("Scene Switched");
    }
}