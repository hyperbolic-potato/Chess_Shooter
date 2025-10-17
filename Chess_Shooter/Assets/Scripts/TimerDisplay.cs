using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    public ScoreKeeper scoreKeeper;
    public TextMeshProUGUI tmp;
    public bool live = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
        if(scoreKeeper != null )
            tmp.text = DisplayClock(scoreKeeper.time);
    }

    // Update is called once per frame
    void Update()
    {
        if(live && scoreKeeper != null) tmp.text = DisplayClock(scoreKeeper.time);
    }

    string DisplayClock(float time) //I can hear you snickering, shut up.
    {
        float seconds = time % 60;
        seconds = (float)(int)(seconds * 100) / 100f;
        int minutes = Mathf.FloorToInt(time / 60);
        int hours = Mathf.FloorToInt(time / 120);
        return hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();

    }
}
