using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class FPC : MonoBehaviour
{
    private CharacterController characterController;
    public float walkSpeed = 5;
    public float mouseSense = 2;
    float verRot;
    private Camera cam;
    public float upDownRange = 80;
    public float sprintSpeed = 10;
    private float currentSpeed;
    public float jumpForce = 5;
    private Vector3 currentMovement;
    private float gravity = 9.81f;
    private Vector3 hitPoint;
    public ParticleSystem impactPS;
    public int particleCount = 20;
    public float pickUpRange = 2;
    public Transform holdPoint;
    public float throwForce = 5;
    private Item heldItem;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    
    void Update()
    {
        Movement();
        MouseLook();
        Sprint();
        Jump();

        if (heldItem != null)
        {
            if(Input.GetMouseButtonDown(1))
            {
                heldItem.Throw(throwForce, cam.transform.forward);
                heldItem = null;
            }
        }


        if (ObjectInFocus()!= null)
        {
            float distanceToObject = Vector3.Distance(cam.transform.position,ObjectInFocus().transform.position);
    
            if (Input.GetMouseButtonDown(0))
            {
                impactPS.transform.position = hitPoint;
                impactPS.Emit(particleCount);
                if (ObjectInFocus().CompareTag("Sphere"))
                {
                    Destroy(ObjectInFocus());
                }

            }

            if (distanceToObject <= pickUpRange && ObjectInFocus().GetComponent<Item>() != null)
            {
               if (Input.GetMouseButtonDown(1))
               {
                heldItem = ObjectInFocus().GetComponent<Item>();
                heldItem.PickUp(cam.transform, holdPoint.position);

               }
            }
        }
        
    }
    

    void Movement()
    {
        float verInput = Input.GetAxis("Vertical");
        float horInput = Input.GetAxis("Horizontal");
        float verSpeed = verInput * currentSpeed;
        float horSpeed = horInput * currentSpeed;

        Vector3 horizontalMovement = new Vector3(horSpeed, 0, verSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;
        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);
    }
    void Jump()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
               currentMovement.y = jumpForce; 
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void Sprint()
    {
         if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }else
        {
            currentSpeed = walkSpeed;
        }
    }

    void MouseLook()
    {
        
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSense;
        transform.Rotate(0, mouseXRotation, 0);
        verRot -= Input.GetAxis("Mouse Y") * mouseSense;
        verRot = Mathf.Clamp(verRot, -upDownRange, +upDownRange);
        cam.transform.localRotation = Quaternion.Euler(verRot, 0, 0);
    
    }

    public GameObject ObjectInFocus()
    {
        GameObject result = null;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            result = hit.transform.gameObject;
            hitPoint = hit.point;
        }

        return result;
    }
}
