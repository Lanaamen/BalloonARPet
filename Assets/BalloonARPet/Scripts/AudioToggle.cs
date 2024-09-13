using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public Button audioButton;  // Referens till Ljud-knappen
    public Text buttonText;     // Referens till text-komponenten för knappen

    private bool isMuted = false;  // Bool för att hålla koll på om ljudet är på eller av

    private void Start()
    {
        // Säkerställer att referensen till audioButton är satt innan man kopplar eventlyssnaren
        if (audioButton != null)
        {
            // Lägger till en lyssnare som anropar ToggleAudio-funktionen när knappen klickas
            audioButton.onClick.AddListener(ToggleAudio);
            UpdateButtonUI(); // Uppdaterar knappens text och ljudstatus vid start
        }
    }

    // Funktion som växlar ljudet mellan på och av
    private void ToggleAudio()
    {
        isMuted = !isMuted; // Växlar tillståndet för ljudet (på och av)
        UpdateButtonUI(); // Uppdaterar knappens UI och ändrar ljudstatus
    }

    // Funktion som uppdaterar knappens text och ljudnivån i spelet
    private void UpdateButtonUI()
    {
        // Uppdaterar texten på knappen beroende på om ljudet är på eller av
        if (buttonText != null)
        {
            buttonText.text = isMuted ? "Off" : "On";
        }

         // Ändrar ljudnivån i spelet baserat på isMuted; 0 om ljudet är av, 1 om det är på
        AudioListener.volume = isMuted ? 0 : 1;
    }
}
