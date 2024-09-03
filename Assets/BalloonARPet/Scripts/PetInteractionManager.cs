using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AugmentedRealityCourse;

public class PetInteractionManager : MonoBehaviour
{
    public static PetInteractionManager Instance; // Singleton instance

    public enum PetState
    {
        Idle,
        Happy,
        Sad,
        Hungry
    }

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

    private MeshRenderer petRenderer; // Renderer of the instantiated pet
    private GameObject currentPet; // Reference to the current pet

    [SerializeField]
    private Text petStateText; // UI for PetState

    [SerializeField]
    private float fadeDuration = 1.0f; // Duration for fade in seconds

    private PetState currentState;

    private Animator petAnimator; // Animator component for handling animations

    [SerializeField]
    private AudioSource petAudioSource; // AudioSource component for sound playback

    public Button happyButton;
    public Button hungryButton;
    public Button sadButton;
    public Button idleButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (petStateText == null)
        {
            DebugManager.Instance.AddDebugMessage("Pet State Text UI component is missing.");
        }

        // Add listeners to the buttons
        if (happyButton != null) happyButton.onClick.AddListener(SetHappyState);
        if (hungryButton != null) hungryButton.onClick.AddListener(SetHungryState);
        if (sadButton != null) sadButton.onClick.AddListener(SetSadState);
        if (idleButton != null) idleButton.onClick.AddListener(SetIdleState);
    }

    // Setters for different states
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

    private void SetPetState(PetState newState)
    {
        currentState = newState;
        UpdatePetStateUI();
        SetMaterialForCurrentState();
        PlayAnimationForCurrentState(); // Play the animation for the new state
        PlaySoundForCurrentState(); // Play the sound for the new state
    }

 // Ange Pet och initialera animationer
    public void SetPet(GameObject pet)
    {
        //DebugManager.Instance.AddDebugMessage("PET : " + pet.name);
        //DebugManager.Instance.AddDebugMessage("CHILD 1st : " + pet.transform.GetChild(0).name);
        currentPet = pet;
        MeshRenderer[] temp = pet.transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
        petRenderer = temp[0];

        // Get the Animator component from the pet
        petAnimator = pet.transform.GetChild(0).GetComponent<Animator>();
        if (petAnimator == null)
        {
            DebugManager.Instance.AddDebugMessage("Pet does not have an Animator component.");
        }

        if (petRenderer == null)
        {
            DebugManager.Instance.AddDebugMessage("Pet does not have a Renderer component.");
        }
        
        // Initialize the function to randomize a state
        SetRandomInitialState();
    }


    private void SetRandomInitialState()
    {
        currentState = (PetState)Random.Range(0, System.Enum.GetValues(typeof(PetState)).Length);
        DebugManager.Instance.AddDebugMessage("Initial state set to: " + currentState);
        UpdatePetStateUI();
        SetMaterialForCurrentState();
        PlayAnimationForCurrentState();
        PlaySoundForCurrentState();
    }

    private void SetMaterialForCurrentState()
    {
        if (petRenderer == null) return;

        DebugManager.Instance.AddDebugMessage("Setting material for state: " + currentState);
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

    private void SetMaterial(Material newMaterial)
    {
        if (petRenderer != null && newMaterial != null)
        {
            StartCoroutine(FadeToMaterial(newMaterial));
            DebugManager.Instance.AddDebugMessage("Material fading to: " + newMaterial.name);
        }
        else
        {
            DebugManager.Instance.AddDebugMessage("Material or Renderer is missing.");
        }
    }

    private IEnumerator FadeToMaterial(Material targetMaterial)
    {
        Color startColor = petRenderer.material.color;
        Color endColor = targetMaterial.color;

        Material currentMaterial = petRenderer.material;

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = Color.Lerp(startColor, endColor, t);
            currentMaterial.color = newColor;
            yield return null;
        }

        currentMaterial.color = endColor;
        petRenderer.material = targetMaterial;
    }

    public void GiveSnack()
    {
        DebugManager.Instance.AddDebugMessage("GiveSnack button pressed.");
        StartCoroutine(GiveSnackRoutine());
    }

    private IEnumerator GiveSnackRoutine()
    {
        if (petRenderer == null) yield break;

        DebugManager.Instance.AddDebugMessage("GiveSnackRoutine started.");
        SetMaterial(snackMaterial);

        PlayAnimation("GiveSnack"); // Play the "GiveSnack" animation
        PlaySound(snackSound); // Play the snack sound

        yield return new WaitForSeconds(4);

        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
        PlayAnimationForCurrentState(); // Play the happy animation after "GiveSnack"
        PlaySoundForCurrentState(); // Play the happy sound
        DebugManager.Instance.AddDebugMessage("GiveSnackRoutine completed.");
    }

    public void Play()
    {
        DebugManager.Instance.AddDebugMessage("Play button pressed.");
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (petRenderer == null) yield break;

        DebugManager.Instance.AddDebugMessage("PlayRoutine started.");
        SetMaterial(playfulMaterial);

        PlayAnimation("Play"); // Play the playing animation
        PlaySound(playfulSound); // Play the playful sound

        yield return new WaitForSeconds(4);

        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
        PlayAnimationForCurrentState(); // Play the happy animation after playing
        PlaySoundForCurrentState(); // Play the happy sound
        DebugManager.Instance.AddDebugMessage("PlayRoutine completed.");
    }

    private void UpdatePetStateUI()
    {
        if (petStateText != null)
        {
            petStateText.text = "Pet State: " + currentState.ToString();
            DebugManager.Instance.AddDebugMessage("Pet state UI updated to: " + currentState);
        }
        else
        {
            DebugManager.Instance.AddDebugMessage("Pet State Text UI component is missing.");
        }
    }

    private void PlayAnimationForCurrentState()
    {
        if (petAnimator == null) return;

        DebugManager.Instance.AddDebugMessage("Playing animation for state: " + currentState);
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

    private void PlaySoundForCurrentState()
    {
        if (petAudioSource == null) return;

        DebugManager.Instance.AddDebugMessage("Playing sound for state: " + currentState);
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

    private void PlayAnimation(string animationName)
    {
        if (petAnimator != null)
        {
            petAnimator.Play(animationName);
            DebugManager.Instance.AddDebugMessage("Playing animation: " + animationName);
        }
        else
        {
            DebugManager.Instance.AddDebugMessage("Animator component is missing.");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (petAudioSource != null && clip != null)
        {
            petAudioSource.PlayOneShot(clip);
            DebugManager.Instance.AddDebugMessage("Playing sound: " + clip.name);
        }
        else
        {
            DebugManager.Instance.AddDebugMessage("AudioSource or AudioClip is missing.");
        }
    }
}
