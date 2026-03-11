using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class FPC : MonoBehaviour
{
    private CharacterController characterController;
    public float walkSpeed = 5;
    public float mouseSense = 2;
    float verRot;
    private Camera cam;
    public float upDownRange = 80;
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
        
    }
    

    void Movement()
    {
        float verInput = Input.GetAxis("Vertical");
        float horInput = Input.GetAxis("Horizontal");
        float verSpeed = verInput * walkSpeed;
        float horSpeed = horInput * walkSpeed;

        Vector3 horizontalMovement = new Vector3(horSpeed, 0, verSpeed);
        horizontalMovement = transform.rotation * horizontalMovement;

        characterController.Move(horizontalMovement * Time.deltaTime);
    }

    void MouseLook()
    {
        
        float mouseXRotation = Input.GetAxis("Mouse X") * mouseSense;
        transform.Rotate(0, mouseXRotation, 0);
        verRot -= Input.GetAxis("Mouse Y") * mouseSense;
        verRot = Mathf.Clamp(verRot, -upDownRange, +upDownRange);
        cam.transform.localRotation = Quaternion.Euler(verRot, 0, 0);
    
    }
}
