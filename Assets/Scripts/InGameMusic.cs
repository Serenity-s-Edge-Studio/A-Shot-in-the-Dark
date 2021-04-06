using System.Collections;
using UnityEngine;

public class InGameMusic : MonoBehaviour
{
    public AudioSource source;
    public Songs[] songs;

    private void Start()
    {
        StartCoroutine(CycleSongs());
    }
    public IEnumerator CycleSongs()
    {
        if (songs.Length > 0)
        {
            while (true)
            {
                int index = Mathf.Min((int)DayNightCycle.instance.PeriodOfDay, songs.Length);
                int clipAmount = songs[index].clips.Length;
                if (songs[index].clips.Length == 0)
                    break;
                AudioClip currentClip = songs[index].clips[Random.Range(0, clipAmount)];
                source.clip = currentClip;
                source.Play();
                yield return new WaitForSeconds(currentClip.length);
            }
        }
    }
}
[System.Serializable]
public class Songs
{
    [SerializeField]
    private string name;
    public AudioClip[] clips;
}
