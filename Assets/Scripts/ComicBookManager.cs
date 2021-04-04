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
    public ComicBookSO[] ClueComic;

    public AudioSource audioSource;

    public GameObject ComicContainer;
    public Image image;
    public TextMeshProUGUI subTitleText;

    private ComicBookSO currentComicBook;
    private int panelIndex = 0;
    private int frameIndex = 0;
    public void PlayComic(ComicType mode, int index)
    {
        switch (mode)
        {
            case ComicType.Scene:
                if (index < SceneComic.Length)
                {
                    OnStartComic.Invoke();
                    startComic(SceneComic[index]);
                }
                break;
            case ComicType.Clue:
                if (index < ClueComic.Length)
                {
                    OnStartComic.Invoke();
                    startComic(SceneComic[index]);
                }
                break;
        }
        
    }
    private void startComic(ComicBookSO panel)
    {
        ComicContainer.SetActive(true);
        panelIndex = 0;
        frameIndex = 0;
        currentComicBook = panel;
        image.sprite = panel.comicPanels[panelIndex].panes[frameIndex].image;
    }
    private void nextComic()
    {
        if (frameIndex == currentComicBook.comicPanels[panelIndex].panes.Length)
        {
            frameIndex = 0;
            panelIndex++;
        }

        if (panelIndex == currentComicBook.comicPanels.Length)
        {
            OnEndComic.Invoke();
            ComicContainer.SetActive(false);
            return;
        }

        ComicFrame frame = currentComicBook.comicPanels[panelIndex].panes[frameIndex];
        image.sprite = frame.image;
        audioSource.PlayOneShot(frame.voiceLine);
        subTitleText.text = frame.subTitle;
    }
    private IEnumerator PlayComicCoroutine(ComicType mode, int index)
    {
        ComicBookSO currentBook = mode == ComicType.Scene ? SceneComic[index] : ClueComic[index];
        ComicContainer.SetActive(true);
        foreach (ComicPanel panel in currentBook.comicPanels)
        {
            foreach (ComicFrame frame in panel.panes)
            {
                image.sprite = frame.image;
                subTitleText.text = frame.subTitle;
                audioSource.PlayOneShot(frame.voiceLine);
                yield return new WaitForSecondsRealtime(frame.voiceLine.length);
            }
        }
        OnEndComic.Invoke();
    }
}
public enum ComicType
{
    Scene,
    Clue
}
