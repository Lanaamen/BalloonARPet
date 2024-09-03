using System.Collections; // Necessary for IEnumerator
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public RectTransform menuPanel; // Assign your menu panel here
    public float slideDuration = 0.5f; // Duration of the slide animation
    private bool isMenuVisible = false; // To track the current state

    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void Start()
    {
        // Calculate off-screen and on-screen positions based on the panel's height
        float panelHeight = menuPanel.rect.height;
        // Set off-screen position to be +408 on the y-axis
        offScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 408);
        // Set on-screen position to be at 0 on the y-axis
        onScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 0);

        // Initially move the panel to the off-screen position
        menuPanel.anchoredPosition = offScreenPosition;
    }

    public void ToggleMenu()
    {
        // Toggle the menu visibility
        isMenuVisible = !isMenuVisible;

        StopAllCoroutines(); // Stop any ongoing animations
        // Start the sliding animation
        StartCoroutine(SlideMenuCoroutine(isMenuVisible ? onScreenPosition : offScreenPosition));
    }

    private IEnumerator SlideMenuCoroutine(Vector2 targetPosition)
    {
        Vector2 startPosition = menuPanel.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            menuPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        menuPanel.anchoredPosition = targetPosition; // Ensure it's exactly at the target position
    }
}
