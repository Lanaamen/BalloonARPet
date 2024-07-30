using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PetInteractionManager : MonoBehaviour
{
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
    private Renderer balloonRenderer; // Assign this via the Inspector
    [SerializeField]
    private Renderer threadRenderer; // Assign this via the Inspector

    [SerializeField]
    private Text petStateText; // Assign this via the Inspector

    private PetState currentState;

    private void Awake()
    {
        // Ensure Renderer components and materials are assigned
        if (balloonRenderer == null)
        {
            Debug.LogError("Balloon Renderer component is missing. Assign it in the Inspector.");
        }
        if (threadRenderer == null)
        {
            Debug.LogError("Thread Renderer component is missing. Assign it in the Inspector.");
        }
        if (idleMaterial == null || happyMaterial == null || sadMaterial == null || hungryMaterial == null ||
            snackMaterial == null || playfulMaterial == null)
        {
            Debug.LogError("One or more materials are missing. Please assign all materials in the Inspector.");
        }

        SetRandomInitialState();
    }

    private void SetRandomInitialState()
    {
        // Set a random initial state
        currentState = (PetState)Random.Range(0, System.Enum.GetValues(typeof(PetState)).Length);
        Debug.Log("Initial state set to: " + currentState);
        UpdatePetStateUI();
        SetMaterialForCurrentState();
    }

    private void SetMaterialForCurrentState()
    {
        // Log the state being set
        Debug.Log("Setting material for state: " + currentState);
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
            default:
                Debug.LogError("Unhandled PetState: " + currentState);
                break;
        }
    }

    private void SetMaterial(Material material)
    {
        if (balloonRenderer == null || threadRenderer == null)
        {
            Debug.LogError("One or more Renderer components are missing.");
            return;
        }
        if (material == null)
        {
            Debug.LogError("Material is missing.");
            return;
        }

        // Check if the material is applied correctly
        Debug.Log("Applying material: " + material.name);
        // Ensure materials are not null
        if (balloonRenderer.material == null || threadRenderer.material == null)
        {
            Debug.LogError("Renderer material is null.");
        }
        balloonRenderer.material = material;
        threadRenderer.material = material;
    }

    public void GiveSnack()
    {
        Debug.Log("GiveSnack button pressed.");
        StartCoroutine(GiveSnackRoutine());
    }

    private IEnumerator GiveSnackRoutine()
    {
        Debug.Log("Starting GiveSnackRoutine.");
        SetMaterial(snackMaterial);
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
    }

    public void Play()
    {
        Debug.Log("Play button pressed.");
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        Debug.Log("Starting PlayRoutine.");
        SetMaterial(playfulMaterial);
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
    }

    private void UpdatePetStateUI()
    {
        if (petStateText != null)
        {
            petStateText.text = "Pet State: " + currentState.ToString();
            Debug.Log("Pet state UI updated to: " + currentState);
        }
        else
        {
            Debug.LogError("Pet State Text UI component is missing.");
        }
    }
}
