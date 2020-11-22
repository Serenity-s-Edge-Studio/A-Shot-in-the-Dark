using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsGO;
    [SerializeField]
    private Toggle muteToggle;
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private Toggle movementToggle;
    [SerializeField]
    private TextMeshProUGUI volumeText;
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(settingsGO);
        updateSettings();
    }
    public void updateSettings()
    {
        AudioListener.volume = muteToggle.isOn ? 0 : volumeSlider.value;
        Player.instance.toggleMovement = movementToggle.isOn;
        volumeText.text = $"Volume: {Mathf.RoundToInt(volumeSlider.value * 100)}/100";
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadSceneAsync(index).completed += GameManager_completed;
    }

    private void GameManager_completed(AsyncOperation obj)
    {
        Button settingsButton = GameObject.FindGameObjectWithTag("SettingsButton").GetComponent<Button>();
        settingsButton.onClick.AddListener(() => settingsGO.SetActive(!settingsGO.activeInHierarchy));
        updateSettings();
    }
}
