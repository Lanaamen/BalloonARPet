using UnityEngine;
using UnityEngine.UI;

public class QuitGameManager : MonoBehaviour
{
    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private GameObject confirmationPanel;

    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    private void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(QuitGame);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(CloseConfirmationPanel);
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    public void OnQuitButtonClick()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void CloseConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }
}
