using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Rigidbody rb;
    Transform camTransform;

    float inputX;
    float inputY;

    float inputRot;
    float inputTilt;
    

    public float speed = 5f;
    public float sensitivity = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camTransform = GetComponent<Camera>().gameObject.transform;
    }

    private void Update()
    {
        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;
        // vvv naive vvv
        Vector3 charRotate = new Vector3(0, inputRot * sensitivity, 0);
        
        transform.Rotate(charRotate);

        Vector3 camTilt += new Vector3(0, 0, 0); //TODO this

        camTransform.localRotation = camTilt;

        // ^^^ naive ^^^
        rb.linearVelocity = (tempMove.x * transform.forward) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.right);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputAxis = context.ReadValue<Vector2>();

        inputX = inputAxis.x;
        inputY = inputAxis.y;
    }

    public void NaiveLook(InputAction.CallbackContext context)
    {
        Vector2 inputAxis = context.ReadValue<Vector2>();

        inputRot = inputAxis.x;

        inputTilt += inputAxis.y;
        inputTilt = Mathf.Clamp(inputTilt, -90, 90); //TODO fix this
    }
}
