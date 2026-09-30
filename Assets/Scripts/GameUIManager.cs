using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private BeatManager beatManager;
    [SerializeField] private WaveManager waveManager;

    private void Start()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        beatManager.StartMusic();
        waveManager.EnableFirstWaveButton();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public bool IsTutorialOpen()
    {
        return tutorialPanel.activeSelf;
    }
}