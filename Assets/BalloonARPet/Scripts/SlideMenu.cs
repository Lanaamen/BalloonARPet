using System.Collections; // Necessary for IEnumerator
using UnityEngine;

public class SlideMenu : MonoBehaviour
{
    public RectTransform menuPanel; // Referens till panelen
    public float slideDuration = 0.5f; // Meny-animationens längd
    private bool isMenuVisible = false; // Bool för menyns synlighet

    // Positionerna för menypanelen när den är på skärmen och utanför skärmen
    private Vector2 offScreenPosition;
    private Vector2 onScreenPosition;

    private void Start()
    {
        // Hämtar panelens höjd
        float panelHeight = menuPanel.rect.height;
        // Definierar positionen där menyn är helt utanför skärmen (off-screen)
        offScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 408);
        // Definierar positionen där menyn är synlig på skärmen (on-screen)
        onScreenPosition = new Vector2(menuPanel.anchoredPosition.x, 0);
        // Sätter menypanelens initiala position till offScreenPosition för att börja utanför skärmen
        menuPanel.anchoredPosition = offScreenPosition;
    }

    // Funktion för att visa eller dölja menyn beroende på dess nuvarande tillstånd
    public void ToggleMenu()
    {
        // Växlar menyns synlighet
        isMenuVisible = !isMenuVisible;
        // Stopppar all pågående animation
        StopAllCoroutines(); 
        // Startar en ny coroutine för att animera menyn till antingen on-screen eller off-screen position
        StartCoroutine(SlideMenuCoroutine(isMenuVisible ? onScreenPosition : offScreenPosition));
    }

    // Coroutine som hanterar animationen för att flytta menyn till en målad position
    private IEnumerator SlideMenuCoroutine(Vector2 targetPosition)
    {
        // Sparar panelens nuvarande position som startposition för animationen
        Vector2 startPosition = menuPanel.anchoredPosition;
        // Variabel för att hålla reda på hur lång tid som har gått sedan animationen började
        float elapsedTime = 0f;

        // Loopar medan animationen pågår (så länge elapsedTime är mindre än slideDuration)
        while (elapsedTime < slideDuration)
        {
            // Interpolerar mellan startPosition och targetPosition baserat på hur lång tid som gått
            menuPanel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            // Uppdaterar elapsedTime med tiden som passerat sedan senaste bildrutan
            elapsedTime += Time.deltaTime;
            // Pausar execution tills nästa bildruta innan loopen körs igen
            yield return null;
        }

        // När loopen är klar, se till att menypanelen är exakt vid målpositionen
        menuPanel.anchoredPosition = targetPosition;
    }
}
