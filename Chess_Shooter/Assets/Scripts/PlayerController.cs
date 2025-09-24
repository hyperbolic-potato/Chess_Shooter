using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public Rigidbody rb;
    

    public Vector3 cameraOffset = new Vector3(0f, .5f, .25f);
    Vector2 cameraRotation = Vector2.zero;
    Camera playerCam;
    public InputAction lookAxis;
    public PlayerInput input;
    public Transform weaponSlot;
    public Weapon currentWeapon;
    Ray interactRay;
    RaycastHit interactHit;
    GameObject pickupObject;

    float inputX;
    float inputY;
    float coyoteTime;
    float iTime;
    
    public float speed = 5f;
    public float sensitivity = .7f;
    public float jumpSpeed = 5f;
    public float maxGroundingSlope = 30f;
    public float jumpGrace = 0.5f;
    public float maxITime = 0.5f;
    public float interactDistance;

    public int maxExtraJumps = 1;
    public int health = 5;
    public int maxHealth = 5;

    int extraJumps = 0;

    bool isJumping;
    bool deltaJump;
    bool isGrounded;

    bool attacking = false;

    void Start()
    {
        input = GetComponent<PlayerInput>();

        interactRay = new Ray(transform.position, transform.forward);

        rb = GetComponent<Rigidbody>();
        playerCam = Camera.main;
        weaponSlot = playerCam.transform.GetChild(0);
        lookAxis = GetComponent<PlayerInput>().currentActionMap.FindAction("Look");

        health = maxHealth;



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

        //Interact

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if(Physics.Raycast(interactRay, out interactHit, interactDistance))
        {
            if(interactHit.collider.tag == "Weapon")
            {
                pickupObject = interactHit.collider.gameObject;
            }
            else
            {
                pickupObject = null;
            }
        }

        if (currentWeapon && currentWeapon.holdToAttack && attacking) currentWeapon.fire();
        //Movement System

        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = inputY * speed;
        tempMove.z = inputX * speed;





        if (isJumping && coyoteTime > 0 && isJumping != deltaJump)
        {
            tempMove.y = jumpSpeed;
            coyoteTime = 0;

        } else if (isJumping && extraJumps > 0 && isJumping != deltaJump)
        {
            tempMove.y = jumpSpeed;
            extraJumps--;
        }


        deltaJump = isJumping;

        rb.linearVelocity = (tempMove.x * transform.forward) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.right);

        //coyote frames

        if (isGrounded)
        {
            coyoteTime = jumpGrace;
            extraJumps = maxExtraJumps;
        }
        else if (coyoteTime > 0)
        {
            coyoteTime -= Time.deltaTime;
        }

        //invulnerability frames
        if(iTime > 0)
        {
            iTime -= Time.deltaTime;
        }


        //health & damage
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //input functions
    public void Attack(InputAction.CallbackContext context)
    {
        if (currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton()) attacking = true; else attacking = false;
            }
            else if (context.ReadValueAsButton())
            {
                currentWeapon.fire();
            }
        }
    }

    public void Reload()
    {
        if (currentWeapon && !currentWeapon.reloading) currentWeapon.reload();
    }

    public void Interact()
    {
        if (pickupObject)
        {
            if (pickupObject.tag == "Weapon")
            {
                if (currentWeapon) DropWeapon();

                pickupObject.GetComponent<Weapon>().equip(this);

                pickupObject = null;
            }
            else Reload();
        }
    }

    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.GetComponent<Weapon>().unequip();
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
        }
    }
}
