using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public Button audioButton;  // Reference to the Button component
    public Text buttonText;     // Reference to the Text component on the button

    private bool isMuted = false;  // State to track whether audio is muted or not

    private void Start()
    {
        // Ensure that references are assigned
        if (audioButton != null)
        {
            audioButton.onClick.AddListener(ToggleAudio);
            UpdateButtonUI();
        }
        else
        {
            Debug.LogError("AudioButton reference is missing.");
        }
    }

    private void ToggleAudio()
    {
        isMuted = !isMuted;          // Toggle the muted state
        UpdateButtonUI();           // Update the button UI and audio state
    }

    private void UpdateButtonUI()
    {
        // Update the button text based on the muted state
        if (buttonText != null)
        {
            buttonText.text = isMuted ? "Off" : "On";
        }
        else
        {
            Debug.LogError("ButtonText reference is missing.");
        }

        // Update audio volume
        AudioListener.volume = isMuted ? 0 : 1;
    }
}
