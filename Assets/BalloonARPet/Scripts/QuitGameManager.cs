using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameManager : MonoBehaviour
{
    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private GameObject confirmationPanel;
    [SerializeField]
    private Button yesButton;
    [SerializeField]
    private Button noButton;

    [SerializeField]
    private PetInteractionManager petInteractionManager; // Reference to PetInteractionManager

    private void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesButtonClick);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoButtonClick);
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false); // Initially hide the confirmation panel
        }
    }

    private void OnQuitButtonClick()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true); // Show the confirmation panel
        }
    }

    private void OnYesButtonClick()
    {
        // Play pop animation, sound, and destroy the balloon via PetInteractionManager
        if (petInteractionManager != null)
        {
            petInteractionManager.PlayPopAnimationAndSound();
        }

        StartCoroutine(QuitGameAfterAnimation());
    }

    private void OnNoButtonClick()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false); // Hide the confirmation panel
        }
    }

    private IEnumerator QuitGameAfterAnimation()
    {
        // Wait for the pop animation, sound, and balloon destruction
        yield return new WaitForSeconds(0.2f); // Adjust based on animation duration

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stop the game in the Unity Editor
        #else
            Application.Quit(); // Quit the game
        #endif
    }
}
