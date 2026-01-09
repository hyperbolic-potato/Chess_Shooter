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
    public Transform itemSlot;
    public Weapon currentWeapon;
    Ray interactRay;
    RaycastHit interactHit;
    GameObject pickupObject;
    
    ItemNode head;
    ItemNode heldItem;
    

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
            if(interactHit.collider.tag == "Weapon" || interactHit.collider.tag == "Item")
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

        if(transform.position.y < -512)
        {
            health = 0;
        }
        //inventory
        if(heldItem == null && head != null)
        {
            heldItem = head;
            heldItem.data.GetComponent<MeshRenderer>().enabled = true;
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
            else if (pickupObject.tag == "Item")
            {

                pickupObject.GetComponent<Rigidbody>().isKinematic = true;
                pickupObject.GetComponent<Collider>().enabled = false;
                pickupObject.GetComponent<MeshRenderer>().enabled = false;
                pickupObject.transform.parent = itemSlot;
                pickupObject.transform.localPosition = Vector3.zero;
                pickupObject.transform.localRotation = Quaternion.identity;
                //the above code bloc stows the object in the player's off hand 

                

                if (head == null)
                {
                    head = new ItemNode(pickupObject);
                }
                else
                {
                    head.Append(new ItemNode(pickupObject));
                }

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
        if (context.performed)
        {

        }
    }

    public void Scroll(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {

            ItemNode oldItem = heldItem;
            ItemNode newItem = null;
            
            if (context.ReadValue<float>() > 0) //assign newItem depending on the scroll direction
            {
                newItem = oldItem.next;
            }
            else
            {
                newItem = oldItem.previous;
            }

            if ( oldItem != null && newItem != null) //disable the old item
            {
                oldItem.data.GetComponent<MeshRenderer>().enabled = false;
            }
            if (newItem != null) //enable the new item
            {
                newItem.data.GetComponent<MeshRenderer>().enabled = true;
            }

            if (newItem != null) heldItem = newItem; //set the currently held item to be the new one
            
        }
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pickupObject.GetComponent<Rigidbody>().isKinematic = false;
            pickupObject.GetComponent<Collider>().enabled = true;
            pickupObject.GetComponent<MeshRenderer>().enabled = true;
            pickupObject.transform.parent = null;
            pickupObject.transform.position = transform.position;
            pickupObject.transform.rotation = Quaternion.identity;

            if(heldItem.previous != null) heldItem.previous.Remove(1); //the main case
            else if(heldItem.next == null) heldItem = null;  //edge case wherin the dropped item is the last item in the inventory
            else            //really obnoxious edge case wherin the dropped item is the head node but there are other items in the list still
            {
                head = heldItem.next;
                head.previous.next = null;
                head.previous = null;
                //In hindsight, I should've made a node class, then a secondary linkedlist class that had all the functions and managed all these weird edge cases
            }
        }
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
        if ((other.tag == "Hazard" || other.tag == "Enemy") && iTime <= 0)
        {
            health--;
            iTime = maxITime;
        }
    }
}
