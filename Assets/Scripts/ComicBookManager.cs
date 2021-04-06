using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComicBookManager : MonoBehaviour
{
    public static ComicBookManager instance;
    private void Awake()
    {
        if (instance) Destroy(instance);
        instance = this;
    }

    public UnityEvent OnStartComic;
    public UnityEvent OnEndComic;

    public ComicBookSO[] SceneComic;

    public AudioSource audioSource;

    public GameObject ComicContainer;
    public Image image;
    public TextMeshProUGUI subTitleText;
    [SerializeField]
    private Button ContinueButton;
    [SerializeField]
    private Button SkipButton;

    private ComicBookSO currentComicBook;
    private int panelIndex = 0;
    private int frameIndex = 0;

    private bool ContinueButtonPressed;
    private void Start()
    {
        ContinueButton.onClick.AddListener(() => ContinueButtonPressed = true);
        SkipButton.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            GameManager.instance.LoadScene(3);
        });
        StartCoroutine(PlayComicCoroutine(0));
    }
    private IEnumerator PlayComicCoroutine(int index)
    {
        ComicBookSO currentBook = SceneComic[index];
        ComicContainer.SetActive(true);
        foreach (ComicPanel panel in currentBook.comicPanels)
        {
            foreach (ComicFrame frame in panel.panes)
            {
                image.sprite = frame.image;
                subTitleText.text = frame.subTitle;
                if (frame.voiceLine != null)
                    audioSource.PlayOneShot(frame.voiceLine);
                float time = Time.time;
                yield return new WaitUntil(() =>
                {
                    return (frame.voiceLine != null ? Time.time - time >= frame.voiceLine.length : false) || ContinueButtonPressed;
                });
                ContinueButtonPressed = false;
            }
        }
        GameManager.instance.LoadScene(3);
    }
}
