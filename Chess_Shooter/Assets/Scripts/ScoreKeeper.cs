using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public int score;
    public float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }
}
