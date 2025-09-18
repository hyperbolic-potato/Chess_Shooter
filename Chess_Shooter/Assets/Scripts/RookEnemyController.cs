using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class RookEnemyController : MonoBehaviour
{

    public NavMeshAgent agent;
    Transform playerTransform;

    public float aggroRadius;
    public float maxAggroTimer;
    public float attackRadius;
    public float attackDelay;
    public float attackCooldown;
    public float attackSpeedMultiplyer;

    float aggroTimer;

    int axis; //0, 1, 2 x y z
    float offset;

    bool isNavigating = true;

    public float maxHealth = 2;

    float health;
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

        Vector3 difference = transform.position - playerTransform.position;


        //aggro
        if (isNavigating)
        {

            if (agent.isStopped && playerTransform.position.magnitude - transform.position.magnitude < aggroRadius)
            {
                agent.isStopped = false;
                switchAxis(difference);
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
        

        //movement on orthagonal axes


        Debug.Log(offset);

        if (agent.remainingDistance < 0.05f)
        {
            switchAxis(difference);
        }


        //attacking
        if (Mathf.Abs(playerTransform.position.magnitude - transform.position.magnitude) < attackRadius && isNavigating)
        {
            StartCoroutine(attack());
        }
    }

    IEnumerator attack()
    {
        yield return null;
    }

    /*IEnumerator redirect()
    {
        while (isNavigating)
        {
            Vector3 difference = playerTransform.position - transform.position;

            Vector3 playerPosX = new Vector3(playerTransform.position.x, 0, 0);
            Vector3 playerPosY = new Vector3(0, playerTransform.position.y, 0);
            Vector3 playerPosZ = new Vector3(0, 0, playerTransform.position.z);

            Vector3 destination = playerPosX;

            if (playerPosY.sqrMagnitude > destination.sqrMagnitude) destination = playerPosY;

            if (playerPosZ.sqrMagnitude > destination.sqrMagnitude) destination = playerPosZ;

            agent.destination = transform.position + destination;
            yield return new WaitForSeconds(attackDelay);
        }
    }*/

    void switchAxis(Vector3 difference)
    {
        if (difference.x > difference.y && difference.x > difference.z) axis = 0;
        if (difference.y > difference.x && difference.y > difference.z) axis = 1;
        if (difference.z > difference.x && difference.z > difference.y) axis = 2;

        switch (axis)
        {
            case 0:
                agent.destination = transform.position + new Vector3(difference.x, 0, 0);
                break;
            case 1:
                agent.destination = transform.position + new Vector3(0, difference.y, 0);
                break;
            case 2:
                agent.destination = transform.position + new Vector3(0, 0, difference.z);
                break;
        }
    }
}
