using System.Collections.Generic;
using TMPro;
#if !UNITY_WEBGL
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

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
    [SerializeField]
    private Button closeSettingsMenu;
    [SerializeField]
    private GameObject LoadingScreen;
    public DifficultySettingsSO SelectedDifficulty;
    public static GameManager instance;
    [SerializeField]
    private int CurrentSceneIndex = 2;
    [SerializeField]
    private Camera persistentCamera;

    private void Awake()
    {
        instance = this;
#if !UNITY_EDITOR
        LoadScene(1, new System.Action<AsyncOperation>[]
        {
            _ => SetLoadButtonListner(2),
            _ => FindAndUpdateSettingsButton()
        }, true);
#endif
    }

    // Start is called before the first frame update
    private void Start()
    {
#if UNITY_EDITOR
        CurrentSceneIndex = SceneManager.GetSceneByBuildIndex(1).isLoaded ? 1 : 2;
        SetLoadButtonListner(2);
        FindAndUpdateSettingsButton();
#else
        updateSettings();
#endif
    }

    public void updateSettings()
    {
        AudioListener.volume = muteToggle.isOn ? 0 : volumeSlider.value;
        Player.instance.toggleMovement = movementToggle.isOn;
        volumeText.text = $"Volume: {Mathf.RoundToInt(volumeSlider.value * 100)}/100";
    }

    public void LoadScene(int index)
    {
        LoadScene(index, null, true);
    }

    public void LoadScene(int index, System.Action<AsyncOperation>[] onComplete, bool setActive)
    {
        LoadingScreen.SetActive(true);
        persistentCamera.gameObject.SetActive(true);
        if (SceneManager.GetSceneByBuildIndex(CurrentSceneIndex).isLoaded)
            SceneManager.UnloadSceneAsync(CurrentSceneIndex);
        AsyncOperation LoadSceneOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        LoadSceneOperation.allowSceneActivation = setActive;
        if (onComplete != null)
        {
            foreach (System.Action<AsyncOperation> operation in onComplete)
            {
                LoadSceneOperation.completed += operation;
            }
        }
        CurrentSceneIndex = index;
        LoadSceneOperation.completed += GameManager_completed;
    }

    private void GameManager_completed(AsyncOperation obj)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(CurrentSceneIndex));
        FindAndUpdateSettingsButton();
        LoadingScreen.SetActive(false);
        persistentCamera.gameObject.SetActive(false);
    }

    private void FindAndUpdateSettingsButton()
    {
        var buttonList = GetAllObjectsOnlyInSceneWithTag<Button>("SettingsButton");
        if (buttonList.Count > 0)
        {
            Button settingsButton = buttonList[0];
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => settingsGO.SetActive(!settingsGO.activeInHierarchy));
            var menu = GetAllObjectsOnlyInSceneWithTag<Transform>("Menu")?.FirstOrDefault();
            if (menu != null)
                closeSettingsMenu.onClick.AddListener(() => menu.gameObject.SetActive(true));
        }
        else
        {
            Debug.LogWarning("Could not find settings button in loaded scenes");
        }
        updateSettings();
    }

    private void SetLoadButtonListner(int index)
    {
        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            var buttonList = GetAllObjectsOnlyInSceneWithTag<Button>("LoadGameButton");
            if (buttonList.Count > 0)
            {
                Button loadGameButton = buttonList[0];
                loadGameButton.onClick.RemoveAllListeners();
                loadGameButton.onClick.AddListener(() => LoadScene(index));
            }
            else
            {
                Debug.LogWarning("Could not find play button in loaded scenes");
            }
        }
    }

    public List<T> GetAllObjectsOnlyInScene<T>() where T : Object
    {
        List<T> objectsInScene = new List<T>();

        foreach (T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (
#if !UNITY_WEBGL
                !EditorUtility.IsPersistent(obj) && //If platform is webgl check if it is not an editor object
#endif
                !(obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave))
            {
                objectsInScene.Add(obj);
            }
        }

        return objectsInScene;
    }

    public List<T> GetAllObjectsOnlyInSceneWithTag<T>(string tag) where T : Component
    {
        List<T> objectsInScene = new List<T>();

        foreach (T obj in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (
#if !UNITY_WEBGL
                !EditorUtility.IsPersistent(obj) && //If platform is webgl check if it is not an editor object
#endif
                !(obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave) && obj.CompareTag(tag))
            {
                objectsInScene.Add(obj);
            }
        }

        return objectsInScene;
    }
}
