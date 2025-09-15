using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Rigidbody rb;
    

    public Vector3 cameraOffset = new Vector3(0f, .5f, .25f);
    Vector2 cameraRotation = Vector2.zero;
    Camera playerCam;
    InputAction lookAxis;

    float inputX;
    float inputY;
    float coyoteTime;
    
    public float speed = 5f;
    public float sensitivity = .7f;
    public float jumpSpeed = 5f;
    public float maxGroundingSlope = 30f;
    public float jumpGrace = 5f;

    bool isJumping;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        lookAxis = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Camera Handler

        playerCam.transform.position = transform.position + cameraOffset;

        cameraRotation.x += lookAxis.ReadValue<Vector2>().x * sensitivity;
        cameraRotation.y += lookAxis.ReadValue<Vector2>().y * sensitivity;

        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90, 90);

        playerCam.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0);
        transform.rotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.up);

        //Quaternion playerRotation = Quaternion.identity;
        //playerRotation.y = playerCam.transform.rotation.y;
        //    transform.rotation = playerRotation;

        //Movement System

        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;

        if (isJumping && coyoteTime > 0)
        {
            tempMove.y = jumpSpeed;
            coyoteTime = 0;
        }

        rb.linearVelocity = (tempMove.x * transform.forward) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.right);

        //coyote frames

        if (isGrounded)
        {
            coyoteTime = jumpGrace;
        }
        else if (coyoteTime > 0)
        {
            coyoteTime -= 1;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputAxis = context.ReadValue<Vector2>();

        inputX = inputAxis.x;
        inputY = inputAxis.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValue<float>() > 0;
    }

    public void OnCollisionStay(Collision collision)
    {

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector2.Angle(collision.GetContact(i).normal, Vector3.up) < maxGroundingSlope) isGrounded = true;
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

}
