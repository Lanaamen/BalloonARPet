using UnityEngine;
using UnityEngine.UI;

public class QuitGameManager : MonoBehaviour
{
    [SerializeField]
    private Button quitButton; // Reference to the UI Button

    private void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("Quit Button is not assigned in the Inspector.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button clicked.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
