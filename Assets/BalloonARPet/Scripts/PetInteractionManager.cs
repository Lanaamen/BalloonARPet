using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PetInteractionManager : MonoBehaviour
{
    public static PetInteractionManager Instance; // Singleton-instans som används för att tillhandahålla en global tillgång till PetInteractionManager

    //Enum för de olika tillstånd ballongen kan ha
    public enum PetState
    {
        Idle,
        Happy,
        Sad,
        Hungry
    }
    
    // Referens till Ballong-materialen för de olika tillstånden
    [SerializeField]
    private Material idleMaterial;
    [SerializeField]
    private Material happyMaterial;
    [SerializeField]
    private Material sadMaterial;
    [SerializeField]
    private Material hungryMaterial;
    [SerializeField]
    private Material snackMaterial;
    [SerializeField]
    private Material playfulMaterial;

    // Referens till Ballong-ljuden
    [SerializeField]
    private AudioClip happySound;
    [SerializeField]
    private AudioClip sadSound;
    [SerializeField]
    private AudioClip hungrySound;
    [SerializeField]
    private AudioClip snackSound;
    [SerializeField]
    private AudioClip playfulSound;
    [SerializeField]
    private AudioClip popSound;

    private MeshRenderer petRenderer; // MeshRenderer för den instansierade ballongen
    private GameObject currentPet; // Referens till de nuvarande husdjuret i appen

    [SerializeField]
    private Text petStateText; // UI-komponent som visar ballongens tillstånd

    [SerializeField]
    private float fadeDuration = 1.0f; // Varighet för övergången mellan materialen

    [SerializeField]
    private AudioSource petAudioSource; // Ljudkomponent som hanterar ljuduppspelning
    private PetState currentState; //Nuvarande tillstånd för ballongen
    private Animator petAnimator; // Animator-komponent som hanterar animationer för ballongen

    // Referens till knapparna som ändrar ballongens tillstånd
    public Button happyButton;
    public Button hungryButton;
    public Button sadButton;
    public Button idleButton;

    // Initierar singleton-mönstret i Awake-metoden
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this; // Om det inte finns en instans, sätt denna till instansen
        }
        else
        {
            Destroy(gameObject); // Om det redan finns en instans, förstör detta objekt
        }

        // Lägger till lyssnare till knapparna för att ändra tillståndet för ballongen
        if (happyButton != null) happyButton.onClick.AddListener(SetHappyState);
        if (hungryButton != null) hungryButton.onClick.AddListener(SetHungryState);
        if (sadButton != null) sadButton.onClick.AddListener(SetSadState);
        if (idleButton != null) idleButton.onClick.AddListener(SetIdleState);
    }

    // Funktioner för att ange de olika tillstånden
    public void SetHappyState()
    {
        SetPetState(PetState.Happy);
    }

    public void SetHungryState()
    {
        SetPetState(PetState.Hungry);
    }

    public void SetSadState()
    {
        SetPetState(PetState.Sad);
    }

    public void SetIdleState()
    {
        SetPetState(PetState.Idle);
    }

    // Huvudfunktion för att ändra husdjurets tillstånd
    private void SetPetState(PetState newState)
    {
        // Stoppar aktuellt ljud innan tillståndet ändras
        StopCurrentSound();

        currentState = newState; // Sätter det nya tillståndet
        UpdatePetStateUI(); // Uppdaterar UI:t med det nya tillståndet
        SetMaterialForCurrentState(); // Sätter rätt material för det nya tillståndet
        PlayAnimationForCurrentState(); // Spelar upp animationen för de nya tillståndet
        PlaySoundForCurrentState(); // Spelar upp ljudet för de nya tillståndet
    }

    // Funktion för att sätta det nuvarande husdjuret
    public void SetPet(GameObject pet)
    {
        currentPet = pet;
        MeshRenderer[] temp = pet.transform.GetChild(0).GetComponentsInChildren<MeshRenderer>(); // Hittar husdjurets MeshRenderer
        petRenderer = temp.Length > 0 ? temp[0] : null; // Om det finns MeshRenderer, använd den första

        // Hämtar Animator och AudioSource-komponenter från husdjuret
        petAnimator = pet.transform.GetChild(0).GetComponent<Animator>();
        petAudioSource = pet.transform.GetChild(0).GetComponent<AudioSource>();

        // Sätter ett slumpmässigt starttillstånd
        SetRandomInitialState();
    }

    // Funktion för att sätta ett slumpmässigt starttillstånd för husdjuret
    private void SetRandomInitialState()
    {
        currentState = (PetState)Random.Range(0, System.Enum.GetValues(typeof(PetState)).Length); // Sätter ett slumpmässigt tillstånd
        UpdatePetStateUI();
        SetMaterialForCurrentState();
        PlayAnimationForCurrentState();
        PlaySoundForCurrentState();
    }

    // Funktion för att sätta material baserat på det aktuella tillståndet
    private void SetMaterialForCurrentState()
    {
        if (petRenderer == null) return; // Om det inte finns någon renderer, avbryt

        switch (currentState)
        {
            case PetState.Idle:
                SetMaterial(idleMaterial);
                break;
            case PetState.Happy:
                SetMaterial(happyMaterial);
                break;
            case PetState.Sad:
                SetMaterial(sadMaterial);
                break;
            case PetState.Hungry:
                SetMaterial(hungryMaterial);
                break;
        }
    }

    // Funktion för att sätta material och eventuellt använda en fade
    private void SetMaterial(Material newMaterial, bool useFade = true)
    {
        if (petRenderer != null && newMaterial != null)
        {
            if (useFade)
            {
                StartCoroutine(FadeToMaterial(newMaterial)); // Startar en fade mellan materialen
            }
            else
            {
                petRenderer.material = newMaterial; // Sätter materialet direkt utan fade
            }
        }
    }

    // Koroutine för att gradvis övergå mellan två material
    private IEnumerator FadeToMaterial(Material targetMaterial)
    {
        Color startColor = petRenderer.material.color; // Startfärg
        Color endColor = targetMaterial.color; // Slutfärg

        Material currentMaterial = petRenderer.material; // Aktuella materialet

        // Gradvis övergång under fadeDuration
        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = Color.Lerp(startColor, endColor, t); // Interpolerar mellan start och slutfärg
            currentMaterial.color = newColor; // Sätter den nya färgen
            yield return null;
        }

        currentMaterial.color = endColor; // Sätter slutfärgen när övergången är klar
        petRenderer.material = targetMaterial; // Sätter det nya materialet
    }

    // Funktion för att ge husdjuret en snack
    public void GiveSnack()
    {
        StartCoroutine(GiveSnackRoutine());
    }

    // Koroutine som hanterar vad som händer när husdjuret får en snack
    private IEnumerator GiveSnackRoutine()
    {
        if (petRenderer == null) yield break; // Om det inte finns någon renderer, avbryt

        // Stoppar eventuella ljud som spelas
        StopCurrentSound();

        // Sätter snack-materialet utan fade
        SetMaterial(snackMaterial, useFade: false);

        PlayAnimation("GiveSnack"); // Spelar upp animationen för att ge snack
        PlaySound(snackSound); // Spelar snack-ljudet

        yield return new WaitForSeconds(4);

       
        SetMaterial(happyMaterial, useFade: false); // Sätter materialet till "Happy" efter snack
        currentState = PetState.Happy; // Ändrar tillståndet till Happy
        UpdatePetStateUI();
        PlayAnimationForCurrentState(); // Spelar upp Happy-animationen
        PlaySoundForCurrentState(); // Spelar upp Happy-ljudet
    }

    // Funktion för att leka med husdjuret
    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }

    // Koroutine som hanterar lekmomentet
    private IEnumerator PlayRoutine()
    {
        if (petRenderer == null) yield break;

        StopCurrentSound();
        SetMaterial(playfulMaterial, useFade: false);
        PlayAnimation("Play"); // Spelar upp play-animationen
        PlaySound(playfulSound); // Spelar upp play-ljudet

        yield return new WaitForSeconds(4);

        // Sätter Happy-materialet efter lek
        SetMaterial(happyMaterial, useFade: false);
        currentState = PetState.Happy;
        UpdatePetStateUI();
        PlayAnimationForCurrentState(); // Spelar happy animationen efter
        PlaySoundForCurrentState(); // Spelar upp happy-ljudet
    }

    // Funktion för att uppdatera UI:t som visar ballongens tillstånd
    private void UpdatePetStateUI()
    {
        if (petStateText != null)
        {
            petStateText.text = "Pet State: " + currentState.ToString();
        }
    }

    // Spelar upp animation baserat på nuvarande tillstånd
    private void PlayAnimationForCurrentState()
    {
        if (petAnimator == null) return;

        switch (currentState)
        {
            case PetState.Idle:
                PlayAnimation("Idle");
                break;
            case PetState.Happy:
                PlayAnimation("Happy");
                break;
            case PetState.Sad:
                PlayAnimation("Sad");
                break;
            case PetState.Hungry:
                PlayAnimation("Hungry");
                break;
        }
    }

    // Spelar upp ljud baserat på nuvarande tillstånd
    private void PlaySoundForCurrentState()
    {
        if (petAudioSource == null) return;

        switch (currentState)
        {
            case PetState.Happy:
                PlaySound(happySound);
                break;
            case PetState.Sad:
                PlaySound(sadSound);
                break;
            case PetState.Hungry:
                PlaySound(hungrySound);
                break;
        }
    }

    // Funktion för att spela en specifik animation
    private void PlayAnimation(string animationName)
    {
        if (petAnimator != null)
        {
            petAnimator.Play(animationName); // Spelar upp den angivna animationen
        }
    }

    // Funktion för att spela ett specifikt ljud
    private void PlaySound(AudioClip clip)
    {
        if (petAudioSource == null || clip == null) return;

        StopCurrentSound(); // Stoppar eventuellt ljud som spelas
        petAudioSource.PlayOneShot(clip); // Spelar upp ljudklippet
    }

    // Stoppar det ljud som för tillfället spelas
    private void StopCurrentSound()
    {
        if (petAudioSource.isPlaying)
        {
            petAudioSource.Stop(); // Stoppar ljudet om något spelas
        }
    }

    // Funktion för att spela upp en pop-animation och ljud
    public void PlayPopAnimationAndSound()
    {
        if (petAnimator != null)
        {
            petAnimator.Play("Pop"); 
        }

        if (petAudioSource != null && popSound != null)
        {
            petAudioSource.PlayOneShot(popSound);
        }

        StartCoroutine(DestroyBalloonAfterAnimation()); // Startar koroutinen för att förstöra ballongen
    }

    // Koroutine som förstör husdjuret efter Pop-animationen
    private IEnumerator DestroyBalloonAfterAnimation()
    {
        yield return new WaitForSeconds(0.2f); // Väntar tills animationen är klar

        if (currentPet != null)
        {
            Destroy(currentPet); // Förstör ballong-objektet
        }
    }
}
