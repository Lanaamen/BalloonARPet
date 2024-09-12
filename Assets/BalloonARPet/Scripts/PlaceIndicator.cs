using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceIndicator : MonoBehaviour
{
    private ARRaycastManager raycastManager; // Referens till ARRaycastManager
    private GameObject indicator; // Referens till indikatorn som visar var objekt kan placeras
    private List<ARRaycastHit> hits = new List<ARRaycastHit> (); // Lista för att lagra träffar från raycast

    void Start()
    {
        // Hittar och sätter referensen till ARRaycastManager-komponenten i scenen
        raycastManager = FindObjectOfType<ARRaycastManager> ();
        // Hämtar det första barnobjektet till detta GameObject och använder det som indikator
        indicator = transform.GetChild(0).gameObject;
        // Inaktiverar indikatorn från början (den visas inte förrän en yta hittas)
        indicator.SetActive (false);
    }

    // Update-metoden körs varje bildruta för att hantera indikatorns placering
    void Update()
    {
        // Skapar en 2D-vector i mitten av skärmen som används för raycast (screen center point)
        var ray = new Vector2(Screen.width / 2, Screen.height / 2);
        // Utför en raycast från skärmens mittpunkt för att hitta AR-plan (ytor)
        if (raycastManager.Raycast(ray, hits, TrackableType.Planes))
        {
            // Hämtar träffens position och rotation (där raycast träffar ett AR-plan)
            Pose hitPose = hits[0].pose;

            // Flyttar indikatorn till träffens position och rotation
            transform.position = hitPose.position;
            transform.rotation = hitPose.rotation;

            // Om indikatorn inte redan visas, aktivera den
            if (!indicator.activeInHierarchy)
            {
                indicator.SetActive(true);
            }
        }
    }
}
