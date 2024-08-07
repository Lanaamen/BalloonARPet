using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private Renderer petRenderer; // Renderer of the instantiated pet
    private GameObject currentPet; // Reference to the current pet

    [SerializeField]
    private Text petStateText; // Assign this via the Inspector

    private PetState currentState;

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
            Debug.LogError("Pet State Text UI component is missing.");
        }
    }

    public void SetPet(GameObject pet)
    {
        currentPet = pet;
        petRenderer = pet.GetComponent<Renderer>();
        if (petRenderer == null)
        {
            Debug.LogError("Pet does not have a Renderer component.");
        }
        SetRandomInitialState();
    }

    private void SetRandomInitialState()
    {
        currentState = (PetState)Random.Range(0, System.Enum.GetValues(typeof(PetState)).Length);
        Debug.Log("Initial state set to: " + currentState);
        UpdatePetStateUI();
        SetMaterialForCurrentState();
    }

    private void SetMaterialForCurrentState()
    {
        if (petRenderer == null) return;

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
        }
    }

    private void SetMaterial(Material material)
    {
        if (petRenderer != null && material != null)
        {
            petRenderer.material = material;
            Debug.Log("Material set to: " + material.name);
        }
        else
        {
            Debug.LogError("Material or Renderer is missing.");
        }
    }

    public void GiveSnack()
    {
        Debug.Log("GiveSnack button pressed.");
        StartCoroutine(GiveSnackRoutine());
    }

    private IEnumerator GiveSnackRoutine()
    {
        if (petRenderer == null) yield break;

        Debug.Log("GiveSnackRoutine started.");
        SetMaterial(snackMaterial);
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
        Debug.Log("GiveSnackRoutine completed.");
    }

    public void Play()
    {
        Debug.Log("Play button pressed.");
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (petRenderer == null) yield break;

        Debug.Log("PlayRoutine started.");
        SetMaterial(playfulMaterial);
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
        Debug.Log("PlayRoutine completed.");
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
