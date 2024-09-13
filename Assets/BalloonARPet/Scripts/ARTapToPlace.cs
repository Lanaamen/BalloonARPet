using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARTapToPlace : MonoBehaviour
{
    [SerializeField]
    private GameObject refToPrefab; //Referens till prefab-objektet (ballongen)

    [SerializeField]
    private ARRaycastManager raycastManager; // Manager för att hantera raycasting i AR

    private static List<ARRaycastHit> hitResults = new List<ARRaycastHit>(); // Lista för att lagra träffar från raycast

    private GameObject spawnedObject; // Referens till det instansierade objektet
    private Camera mainCamera;
    private InputAction touchAction; // InputAction för att hantera touch-input

    private void Awake()
    {
        mainCamera = Camera.main;

        // Initiera InputAction för touch-input och aktivera den
        touchAction = new InputAction(binding: "<Touchscreen>/primaryTouch/position");
        touchAction.Enable();
    }

    private void OnDestroy()
    {
        // Avaktivera InputAction när skriptet förstörs
        touchAction.Disable();
        if (touchAction != null)
        {
            touchAction = null;
        }
    }

    private bool TryGetTouchPosition(out Vector2 touchPos)
    {
        // Använd InputAction för att läsa av touch-positionen på skärmen
        if (touchAction.triggered)
        {
            touchPos = touchAction.ReadValue<Vector2>();
            return true;
        }

        touchPos = default;
        return false;
    }

    private void Update()
    {
        // Kontrollera om det finns touch-input och hämta touch-positionen
        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }

        // Utför en raycast från touch-positionen för att detektera AR-plan
        if (raycastManager.Raycast(touchPos, hitResults, TrackableType.Planes))
        {
            // Hämta posen för träffpunkten på AR-planet
            Pose hitPose = hitResults[0].pose;

            if (spawnedObject == null)
            {
                // Om inget objekt är instansierat, skapa prefaben vid träffpunkten
                spawnedObject = Instantiate(refToPrefab, hitPose.position, hitPose.rotation);
                spawnedObject.SetActive(true);
                PetInteractionManager.Instance.SetPet(spawnedObject); // Informera PetInteractionManager
            }
            else
            {
                // Om ett objekt redan är instansierat, flytta det till den nya positionen, detta istället för att skapa fler
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}
