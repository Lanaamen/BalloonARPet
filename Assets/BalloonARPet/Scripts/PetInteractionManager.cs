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

    private MeshRenderer petRenderer; // Renderer of the instantiated pet
    private GameObject currentPet; // Reference to the current pet

    [SerializeField]
    private Text petStateText; //UI texten för PetState

    [SerializeField]
    private float fadeDuration = 1.0f; // Duration of the fade in seconds

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
            DebugManager.Instance.AddDebugMessage("Pet State Text UI component is missing.");
        }
    }

    //Anger Balongen som pet
    public void SetPet(GameObject pet)
    {
        currentPet = pet;
        MeshRenderer[] temp = pet.GetComponentsInChildren<MeshRenderer>();
        petRenderer = temp[0];
        if (petRenderer == null)
        {
            DebugManager.Instance.AddDebugMessage("Pet does not have a Renderer component.");
        }
        //initierar funktionen som randomiserar ett state
        SetRandomInitialState();
    }

    private void SetRandomInitialState()
    {
        currentState = (PetState)Random.Range(0, System.Enum.GetValues(typeof(PetState)).Length);
        DebugManager.Instance.AddDebugMessage("Initial state set to: " + currentState);
        UpdatePetStateUI();
        SetMaterialForCurrentState();
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
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
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
        yield return new WaitForSeconds(2);
        SetMaterial(happyMaterial);
        currentState = PetState.Happy;
        UpdatePetStateUI();
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
}
