using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActorScript : MonoBehaviour
{
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;

    public Transform eyeLineCam;
    public Rigidbody rb;
    public Collider cc;

    public float movementAcceleration = 100f;
    public float horizontalTopSpeed = 100f;

    public float lookSpeed = 0.2f;

    float verticalLook = 0;

    bool isGrounded = false;
    public float maxGroundingSlope = 30;

    public float jumpForce = 100f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");

        eyeLineCam = GetComponentInChildren<Camera>().gameObject.transform;

        if (eyeLineCam == null)
        {
            Debug.LogError( this.gameObject + " has no camera and is therefore blind why would you do this to her :(");
        }

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //input polling
        Vector2 moveValue = moveAction.ReadValue<Vector2>().normalized * movementAcceleration;
        Vector2 lookValue = lookAction.ReadValue<Vector2>() * lookSpeed;
        bool jumpValue = jumpAction.ReadValue<float>() > 0;

        //moving
        Vector2 horizontalVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude < horizontalTopSpeed)
        {
            rb.AddRelativeForce(new Vector3(moveValue.x, 0, moveValue.y));
        }
        
        //looking

        //splitting and manipulating camera tilt individually because quaternions are screwy

        verticalLook -= lookValue.y;
        verticalLook = Mathf.Clamp(verticalLook, -90, 90);

        eyeLineCam.localEulerAngles = new Vector3(verticalLook, 0, 0);
        transform.Rotate(new Vector3(0, lookValue.x, 0));

        //gross. anyways
        
        //jumping
        
        if(isGrounded && jumpValue)
        {
            rb.AddRelativeForce(new Vector3(0, jumpForce, 0));
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = false;
        for(int i = 0; i < collision.contactCount; i++)
        {
            isGrounded = Vector3.Angle(collision.GetContact(i).normal, Vector3.up) < maxGroundingSlope;

            if (isGrounded) break;
        }
    }

    private void OnCollisionExit(Collision collision) //this might come back to bite me later
    {
        isGrounded = false;
    }
}
