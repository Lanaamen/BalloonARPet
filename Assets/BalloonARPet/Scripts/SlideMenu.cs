using System.Collections; // Necessary for IEnumerator
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public RectTransform menuPanel; // Referens till panelen
    public float slideDuration = 0.5f; // Meny-animationens längd
    private bool isMenuVisible = false; // Bool för menys tillstånd

    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void Start()
    {
        // Kalkylerar off-screen och on-screen positionen baserat på panelens höjd
        float panelHeight = menuPanel.rect.height;
        // Sätter off-screen positionen till +408 på y-axeln
        offScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 408);
        // Sätter on-screen positionen till 0 på y-axlen
        onScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 0);

        // Flyttar menypanelen till offscreenPosition till en början
        menuPanel.anchoredPosition = offScreenPosition;
    }

    public void ToggleMenu()
    {
        // Växlar menyns synlighet
        isMenuVisible = !isMenuVisible;

        StopAllCoroutines(); // Stopppar all pågående animation
        // Påbörjar meny-animationen
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

        menuPanel.anchoredPosition = targetPosition; // Försäkrar att panelen är exakt på målets position
    }
}
