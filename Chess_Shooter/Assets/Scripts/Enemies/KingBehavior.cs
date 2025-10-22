using UnityEngine;
using System.Collections;
using Unity.AI;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
public class KingBehavior : BasicEnemyController
{

    public float runawayDistance = 5f;

    public GameObject vfx;
    public GameManager gm;

    bool isDead = false;

    protected override IEnumerator attack(){
        yield return null;
    }


    private void Start()
    {
        vfx = transform.GetChild(0).gameObject;
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        base.Start();
    }

    protected override void Navigate()
    {
        base.Navigate();

        if (isNavigating)
        {
            Vector3 difference = playerTransform.position - transform.position;

            difference = -difference.normalized;

            agent.destination = transform.position + difference * runawayDistance;
        }
    }

    protected override void Death() //mmm yes my favorite kind of death
    {


        //if(!isDead) StartCoroutine(Checkmate());
        if (!isDead)
        {
            isDead = true;
            StartCoroutine(gm.fadeOut(SceneManager.GetActiveScene().buildIndex + 1));
            vfx.SetActive(false);
            GameObject part = Instantiate(kill, null);
            part.transform.position = transform.position;
            part = null;

        }

        

        
    }

    protected override void DamageUpdate(Collider other)
    {
        if (other.tag == "Respawn")
        {
            health = 0;
        }

        if (other.tag == "PlayerDamage" && iTime <= 0)
        {
            health--;
            iTime = maxITime;
            agent.isStopped = false;
            GameObject part = Instantiate(p, null);
            part.transform.position = transform.position;
            part = null;
            audio.Play();
        }
    }

    IEnumerator Checkmate()
    {
        isDead = true;

        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
