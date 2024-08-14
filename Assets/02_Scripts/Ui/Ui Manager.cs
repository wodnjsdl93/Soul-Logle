using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;
    public Button noneButton;

    void Start()
    {
        startButton.onClick.AddListener(() => OnStartButtonClick());
        settingButton.onClick.AddListener(() => OnStartButtonClick());
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Enemy");
        SceneManager.LoadScene("Player", LoadSceneMode.Additive);
    }
    public void OnsettingButtonClick()
    {
        SceneManager.LoadScene("Setting");
    }
    public void OnNoneButtonClick()
    {
        SceneManager.LoadScene("Start");
    }
}

