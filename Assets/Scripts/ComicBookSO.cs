using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Comic Book")]
public class ComicBookSO : ScriptableObject
{
    public ComicPanel[] comicPanels;
}
[System.Serializable]
public class ComicPanel
{
    public ComicFrame[] panes;
}
[System.Serializable]
public class ComicFrame
{
    public Sprite image;
    public AudioClip voiceLine;
    public string subTitle;
}
