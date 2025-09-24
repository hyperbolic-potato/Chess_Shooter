using UnityEngine;
using System.Collections;
public class BombProjectile : MonoBehaviour
{
    public float fuzeTimer = 2.5f;
    public GameObject explosion;
    void Start()
    {
        StartCoroutine(timedDetonation());

    }

    IEnumerator timedDetonation()
    {
        yield return new WaitForSeconds(fuzeTimer);
        GameObject p = GameObject.Instantiate(explosion, null);
        p.transform.position = transform.position;
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            GameObject p = GameObject.Instantiate(explosion, null);
            p.transform.position = transform.position;
            Destroy(this.gameObject);
        }
    }
}
