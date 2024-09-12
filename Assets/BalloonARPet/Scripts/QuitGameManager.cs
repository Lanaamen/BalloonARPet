using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameManager : MonoBehaviour
{
    [SerializeField]
    private Button quitButton; // Referens till knappen som öppnar bekräftelsepanelen
    [SerializeField]
    private GameObject confirmationPanel; // Referens till bekräftelsepanelen (frågar om man vill avsluta)
    [SerializeField]
    private Button yesButton; // Referens till "Yes"-knappen på bekräftelsepanelen
    [SerializeField]
    private Button noButton; // Referens till "No"-knappen på bekräftelsepanelen

    [SerializeField]
    private PetInteractionManager petInteractionManager; // Referens till PetInteractionManager för att hantera ballongens pop-animering

    private void Start()
    {
        // Om quitButton har tilldelats, lägg till en lyssnare för att hantera klickhändelsen
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        // Om yesButton har tilldelats, lägg till en lyssnare för att hantera klickhändelsen
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesButtonClick);
        }

        // Om noButton har tilldelats, lägg till en lyssnare för att hantera klickhändelsen
        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoButtonClick);
        }

        // Döljer bekräftelsepanelen från början
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false); // Panelen visas inte förrän spelaren trycker på "Quit"-knappen
        }
    }

    // Metod som anropas när "Quit"-knappen klickas
    private void OnQuitButtonClick()
    {
        // Visa bekräftelsepanelen om den finns tillgänglig
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    // Metod som anropas när "Yes"-knappen klickas (spelaren bekräftar att de vill avsluta)
    private void OnYesButtonClick()
    {
        // Anropa PetInteractionManager för att spela pop-animering och ljud, samt ta bort ballongen
        if (petInteractionManager != null)
        {
            petInteractionManager.PlayPopAnimationAndSound();
        }

        // Startar coroutine som väntar på att ballongen ska försvinna innan spelet avslutas
        StartCoroutine(QuitGameAfterAnimation());
    }

    // Metod som anropas när "No"-knappen klickas (spelaren avbryter avslutningen)
    private void OnNoButtonClick()
    {
        // Dölj bekräftelsepanelen igen
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    // Coroutine som väntar på att ballongens pop-animering och ljud ska spelas klart innan spelet avslutas
    private IEnumerator QuitGameAfterAnimation()
    {
        
        yield return new WaitForSeconds(0.2f); 
        // Om vi kör i Unity Editor stannar vi bara simuleringen
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stoppa spelet i Unity Editor
        #else
            Application.Quit(); // Avsluta spelet om det körs utanför Unity Editor
        #endif
    }
}
