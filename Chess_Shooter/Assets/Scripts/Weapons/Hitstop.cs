using UnityEngine;
using System.Collections;

public class Hitstop : MonoBehaviour
{
    public float stopTime = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            StartCoroutine(Stop());
        }
    }


    IEnumerator Stop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(stopTime);
        Time.timeScale = 1f;
    }
}
