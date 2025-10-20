using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
public class RigidbodyRookController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 difference;
    Transform playerTransform;

    

    Vector3 axis;
    Vector3 deltaPos;
    Vector3 emmissionPoint;
    public GameObject bomb;
    public GameObject p;
    public GameObject kill;
    public AudioSource audio;


    public float speed = 5;
    public float delay = 1;
    public float aggroRadius;
    public float maxAggroTimer;
    public float maxITime;
    public float bombDelay;
    public float bombForce;

    public int health = 5;
    

    float aggroTimer;
    float iTime;


    bool redirecting;
    bool isWalled;
    bool isNavigating;
    bool isBombing;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("PlayerActor").transform;

        emmissionPoint = transform.GetChild(0).localPosition;

        difference = playerTransform.position - transform.position;

        audio = GetComponent<AudioSource>();

        axis = GetLargestAxis(difference);
    }

    // Update is called once per frame
    void Update()
    {

        //aggro
        
        
            

            if (!isNavigating && (playerTransform.position - transform.position).magnitude < aggroRadius)
            {
            //Debug.Log("STOP. You violated the law. Pay the courts a fine or serve your sentence.");
                isNavigating = true;

            }



            if (isNavigating)
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
                    isNavigating = false;
                    //Debug.Log("Must've been the wind.");
                }
            }

        if (isNavigating)
        {
            rb.useGravity = false;

            difference = playerTransform.position - transform.position;


            if (!redirecting && Vector3.Scale(axis, difference).magnitude < 0.5f)
            {
                rb.linearVelocity = Vector3.zero;
                StartCoroutine(redirect(difference));

            }
            if (!redirecting)
            {
                MoveAlongAxis(Vector3.Scale(axis, difference).normalized, speed);

            }


            
            

            deltaPos = transform.position;

            //attacking
            if (axis != Vector3.up && axis != Vector3.zero && !redirecting && !isBombing)
            {
                StartCoroutine(Bomb());
            }
        }
        else
        {
            rb.useGravity = true;
        }
        
        

        if(health <= 0)
        {
            GameObject part = Instantiate(kill, null);
            part.transform.position = transform.position;
            part = null;
            Destroy(this.gameObject);
        }
        if (iTime > 0) iTime -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        //Debug.Log(Physics.Raycast(new Ray(transform.position, difference)));
    }

    Vector3 GetLargestAxis(Vector3 difference)
    {

        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y) && Mathf.Abs(difference.x) > Mathf.Abs(difference.z)) return Vector3.right;
        if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x) && Mathf.Abs(difference.y) > Mathf.Abs(difference.z)) return Vector3.up;
        else return Vector3.forward;
    }

    void MoveAlongAxis(Vector3 axis, float speed)
    {
        rb.linearVelocity = axis * speed;
    }

    IEnumerator redirect(Vector3 diff)
    {
        Debug.Log("redirecting...");
        redirecting = true;
        yield return new WaitForSeconds(delay);
        axis = GetLargestAxis(diff);
        difference = playerTransform.position - transform.position;
        redirecting = false;
    }

    IEnumerator Bomb()
    {
        isBombing = true;
        
        GameObject b = Instantiate(bomb, null);
        b.transform.position = transform.position + emmissionPoint;
        b.GetComponent<Rigidbody>().linearVelocity = Vector3.up * bombForce;
        yield return new WaitForSeconds(bombDelay);
        isBombing = false;

    }

    private void OnCollisionStay(Collision collision)
    {
        isWalled = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (Mathf.Abs(collision.contacts[i].normal.y) < 0.05f)
            {
                isWalled = true;
                
            }
        }
        

        // if this works first try I'm converting to Islam.

        // INSH'ALLAH
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
        if (other.tag == "PlayerDamage" && iTime <= 0)
        {
            health--;
            iTime = maxITime;
            isNavigating = true;
            GameObject part = Instantiate(p, null);
            part.transform.position = transform.position;
            part = null;
            audio.Play();
        }
    }
}
