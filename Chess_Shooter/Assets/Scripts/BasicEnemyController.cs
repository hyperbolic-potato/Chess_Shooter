using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class BasicEnemyController : MonoBehaviour
{

    public NavMeshAgent agent;
    Transform playerTransform;

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
    float iTime;

    bool isNavigating = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("PlayerActor").transform;
        agent.isStopped = true;

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //aggro
        if (isNavigating)
        {
            agent.destination = playerTransform.position;

            if (agent.isStopped && playerTransform.position.magnitude - transform.position.magnitude < aggroRadius)
            {
                agent.isStopped = false;
                //Debug.Log("STOP. You violated the law. Pay the courts a fine or serve your sentence.");

            }



            if (!agent.isStopped)
            {
                if (playerTransform.position.magnitude - transform.position.magnitude > aggroRadius)
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

        

        //attacking
        if (Mathf.Abs(playerTransform.position.magnitude - transform.position.magnitude) < attackRadius && isNavigating)
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
            Destroy(this.gameObject);
        }

    }

    IEnumerator attack()
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

    private void DamageUpdate(Collider other)
    {
        if (other.tag == "Respawn")
        {
            health = 0;
        }
        if (other.tag == "Health" && health < maxHealth)
        {
            health++;
            Destroy(other.gameObject);
        }
        if (other.tag == "Hazard" && iTime <= 0)
        {
            health--;
            iTime = maxITime;
            agent.isStopped = false;
        }
    }
}
