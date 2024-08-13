using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(() => OnStartButtonClick());
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Enemy");
        SceneManager.LoadScene("Player", LoadSceneMode.Additive);
    }
}
