using UnityEngine;
using System.Collections;
public class RigidbodyRookController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 difference;
    Transform playerTransform;

    Vector3 axis;
    Vector3 deltaPos;

    public float speed = 5;
    public float delay = 1;

    bool redirecting;
    bool isWalled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("PlayerActor").transform;

        difference = playerTransform.position - transform.position;

        axis = GetLargestAxis(difference);
    }

    // Update is called once per frame
    void Update()
    {
        difference = playerTransform.position - transform.position;
        if (Vector3.Scale(axis, difference).magnitude < 0.5f)
        {
            rb.linearVelocity = Vector3.zero;
            StartCoroutine(redirect(difference));
            
        }
        if (!redirecting)
        {
            MoveAlongAxis(Vector3.Scale(axis, difference).normalized, speed);

        }


        if (!redirecting && isWalled)
        {
            axis = Vector3.up;
        }

        deltaPos = transform.position;
        

        
    }

    private void FixedUpdate()
    {
        Debug.Log(Physics.Raycast(new Ray(transform.position, difference)));
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

    IEnumerator redirect(Vector3 difference)
    {
        redirecting = true;
        yield return new WaitForSeconds(delay);
        axis = GetLargestAxis(difference);
        redirecting = false;
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
}
