using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class BasicEnemyController : MonoBehaviour
{

    public NavMeshAgent agent;
    protected Transform playerTransform;
    public GameObject p;
    public GameObject kill;
    public AudioSource audio;

    public float aggroRadius;
    public float maxAggroTimer;
    public float attackRadius;
    public float attackDelay;
    public float attackCooldown;
    public float attackSpeedMultiplyer;
    public float maxITime = 0.5f;

    float aggroTimer;
    public int health = 2;
    public int maxHealth = 2;
    protected float iTime;

    protected bool isNavigating = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("PlayerActor").transform;
        agent.isStopped = true;

        audio = GetComponent<AudioSource>();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        Navigate();
        

        //attacking
        if ((playerTransform.position - transform.position).magnitude < attackRadius && isNavigating)
        {
            //initiating first part of the attack (sidestep)
            StartCoroutine(attack());
        }

        //invulnerability frames
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
        }


        //health & damage
        if (health <= 0)
        {
            Death();
        }

    }

    protected virtual IEnumerator attack()
    {
        //initiating first part of the attack (sidestep)
        agent.speed *= attackSpeedMultiplyer;
        isNavigating = false;
        agent.isStopped = true;
        

        Vector3 difference = playerTransform.position - transform.position;

        // there's gotta be a better way to do this
        int direction = Random.Range(0, 2);

        if(direction == 0)
        {
            direction = -1;
        }

        Vector3 sidestepPos = Quaternion.AngleAxis(45f * direction, Vector3.up) * difference * Mathf.Sqrt(2) / 2; //im so good at math look at me :>

        agent.destination = transform.position + sidestepPos;
        agent.isStopped = false;

        yield return new WaitForSeconds(attackDelay);

        //initiating the 2nd part

        

        agent.destination = transform.position + Quaternion.AngleAxis(90f * -direction, Vector3.up) * sidestepPos;

        yield return new WaitForSeconds(attackCooldown);

        

        agent.speed /= attackSpeedMultiplyer;
        isNavigating = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageUpdate(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        DamageUpdate(other.collider);
    }

    protected virtual void DamageUpdate(Collider other)
    {
        if (other.tag == "Respawn")
        {
            health = 0;
        }

        if ((other.tag == "Hazard" || other.tag == "PlayerDamage") && iTime <= 0)
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

    protected virtual void Navigate()
    {
        //aggro
        if (isNavigating)
        {
            agent.destination = playerTransform.position;

            if (agent.isStopped && (playerTransform.position - transform.position).magnitude < aggroRadius)
            {
                agent.isStopped = false;
                //Debug.Log("STOP. You violated the law. Pay the courts a fine or serve your sentence.");

            }



            if (!agent.isStopped)
            {
                if ((playerTransform.position - transform.position).magnitude > aggroRadius)
                {
                    aggroTimer -= Time.deltaTime;
                }
                else
                {
                    aggroTimer = maxAggroTimer;
                }

                if (aggroTimer <= 0)
                {
                    agent.isStopped = true;
                    //Debug.Log("Must've been the wind.");
                }
            }
        }
    }

    protected virtual void Death()
    {
        GameObject part = Instantiate(kill, null);
        part.transform.position = transform.position;
        part = null;
        Destroy(this.gameObject);
    }
}
