using UnityEngine;
using System.Collections;
public class BombProjectile : MonoBehaviour
{
    public float fuzeTimer = 2.5f;
    public float impactFuze = 0.2f;
    public GameObject explosion;

    bool isTriggered;
    void Start()
    {
        StartCoroutine("timedDetonation", fuzeTimer);
        isTriggered = false;
    }

    IEnumerator timedDetonation(float fuze)
    {
        
        yield return new WaitForSeconds(fuze);
        GameObject p = GameObject.Instantiate(explosion, null);
        p.transform.position = transform.position;
        p.tag = this.tag;
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            isTriggered = true;
            StartCoroutine("timedDetonation", 0f);
        }
        else if (collision.collider.tag == "Environment" && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine("timedDetonation", impactFuze);
        }
    }
}
