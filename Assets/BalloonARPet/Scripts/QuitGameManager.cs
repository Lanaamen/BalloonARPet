using UnityEngine;
using UnityEngine.UI;

public class QuitGameManager : MonoBehaviour
{
    [SerializeField]
    private Button quitButton; // Referens till UI-knappen(quit)

    private void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame); // Lägg till en lyssnare för knapptryckningen
        }
       
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Stoppa spelet i Unity Editor
        #else
            Application.Quit(); 
        #endif
    }
}
