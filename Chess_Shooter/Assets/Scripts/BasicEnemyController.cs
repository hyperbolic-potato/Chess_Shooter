using UnityEngine;
using UnityEngine.AI;
public class BasicEnemyController : MonoBehaviour
{

    public NavMeshAgent agent;
    Transform playerTransform;

    public float aggroRadius;
    public float maxAggroTimer;
    public float attackRadius;

    float aggroTimer;

    bool isNavigating = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("PlayerActor").transform;
        agent.isStopped = true;
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
                Debug.Log("STOP. You violated the law. Pay the courts a fine or serve your sentence.");

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
                    Debug.Log("Must've been the wind.");
                }
            }
        }
        

        //attacking
        if(playerTransform.position.magnitude - transform.position.magnitude < attackRadius && isNavigating)
        {
            agent.isStopped = true;
            isNavigating = false;
            Debug.Log("So be it. *draws sword*");

            Vector3 difference = playerTransform.position - transform.position;


            Vector3 sidestepPos = Quaternion.AngleAxis(45f, Vector3.up) * difference * Mathf.Sqrt(2) / 2; //im so good at math look at me :>

            agent.destination = transform.position + sidestepPos;
            agent.isStopped = false;
        }
    }


}
