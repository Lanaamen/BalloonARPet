using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PetInteractionManager : MonoBehaviour

{   // Singleton-instans för tillgång till PetInteractionManager
    public static PetInteractionManager Instance;

    //Enum ballongens olika tillstånd
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
    private Text petStateText; // Text för ballongens nuvarande tillstånd

    [SerializeField]
    private float fadeDuration = 1.0f; // Varighet för övergången mellan materialen

    [SerializeField]
    private AudioSource petAudioSource; 
    private PetState currentState; 
    private Animator petAnimator; 

    // Referens till knapparna som ändrar ballongens tillstånd
    public Button happyButton;
    public Button hungryButton;
    public Button sadButton;
    public Button idleButton;

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

        currentState = newState;
        UpdatePetStateUI();
        SetMaterialForCurrentState();
        PlayAnimationForCurrentState();
        PlaySoundForCurrentState(); 
    }

    // Funktion för att sätta det nuvarande husdjuret
    public void SetPet(GameObject pet)
    {
        currentPet = pet;
        MeshRenderer[] temp = pet.transform.GetChild(0).GetComponentsInChildren<MeshRenderer>(); // Hittar ballongens MeshRenderer
        petRenderer = temp.Length > 0 ? temp[0] : null; // Om det finns MeshRenderer, använd den första

        // Hämtar Animator och AudioSource-komponenter från ballongen
        petAnimator = pet.transform.GetChild(0).GetComponent<Animator>();
        petAudioSource = pet.transform.GetChild(0).GetComponent<AudioSource>();

        SetRandomInitialState();
    }

    // Funktion för att sätta ett slumpmässigt starttillstånd för ballongen
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

    // coroutine för att gradvis övergå mellan två material
    private IEnumerator FadeToMaterial(Material targetMaterial)
    {
        Color startColor = petRenderer.material.color;
        Color endColor = targetMaterial.color;

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

    // Funktion för att ge ballongen en snack
    public void GiveSnack()
    {
        StartCoroutine(GiveSnackRoutine());
    }

    // coroutine som hanterar vad som händer när ballongen får en snack
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
        PlayAnimationForCurrentState();
        PlaySoundForCurrentState();
    }

    // Funktion för att leka med hballongen
    public void Play()
    {
        StartCoroutine(PlayRoutine());
    }

    // coroutine som hanterar lekmomentet
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
        PlayAnimationForCurrentState();
        PlaySoundForCurrentState();
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

        // Startar coroutinen för att förstöra ballongen
        StartCoroutine(DestroyBalloonAfterAnimation()); 
    }

    // Coroutine som förstör ballongen efter Pop-animationen
    private IEnumerator DestroyBalloonAfterAnimation()
    {
        yield return new WaitForSeconds(0.2f); // Väntar tills animationen är klar

        if (currentPet != null)
        {
            Destroy(currentPet); // Förstör ballong-objektet
        }
    }
}
