using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class ARTapToPlace : MonoBehaviour
{
    [SerializeField]
    private GameObject refToPrefab; // The prefab to instantiate in AR

    [SerializeField]
    private ARRaycastManager raycastManager; // AR Raycast Manager for detecting planes

    private static List<ARRaycastHit> hitResults = new List<ARRaycastHit>(); // List to store raycast hits

    private GameObject spawnedObject; // Reference to the currently spawned object

    private Camera mainCamera; // Main camera reference

    private InputAction touchAction; // Input action for touch input

    private void Awake()
    {
        mainCamera = Camera.main; // Get the main camera

        // Initialize the InputAction for touch input and enable it
        touchAction = new InputAction(binding: "<Touchscreen>/primaryTouch/position");
        touchAction.Enable();
    }

    private void OnDestroy()
    {
        // Disable the InputAction when the script is destroyed
        touchAction.Disable();
        if (touchAction != null)
        {
            touchAction = null;
        }
    }

    private bool TryGetTouchPosition(out Vector2 touchPos)
    {
        // Use InputAction to read touch position on the screen
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
        // Check if there is a touch input and get the touch position
        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }

        // Perform a raycast from the touch position to detect AR planes
        if (raycastManager.Raycast(touchPos, hitResults, TrackableType.Planes))
        {
            // Get the pose of the hit point on the AR plane
            Pose hitPose = hitResults[0].pose;

            if (spawnedObject == null)
            {
                // If no object has been spawned yet, instantiate the prefab at the hit point
                spawnedObject = Instantiate(refToPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                // If an object has already been spawned, move it to the new position
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}
