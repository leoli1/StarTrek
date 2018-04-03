using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerV2 : MonoBehaviour {

    CharacterController controller;

    [SerializeField]
    private float walkSpeed = 6f;
    [SerializeField]
    private float runSpeed = 11f;
    [SerializeField]
    private float jumpSpeed = 4f;
    [SerializeField]
    private float gravity = 9.81f;

    private Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    private GameObject eyes;
    [SerializeField]
    private float sensitivity = 2f;
    private float rotX;
    private float rotY;

    private float moveFB;
    private float moveLR;
    private float moveUD;

    private float speed;

    private bool mouseLocked = true;

    void Start () {
        controller = GetComponent<CharacterController>();
    }
	
	void FixedUpdate () {
        //Bewegungsvariablen
        moveFB = Input.GetAxisRaw("Horizontal") * speed;
        moveLR = Input.GetAxisRaw("Vertical") * speed;

        rotX = Input.GetAxisRaw("Mouse X") * sensitivity;
        rotY -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, -89f, 89f);

        eyes.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
        transform.Rotate(0, rotX, 0);   //dreht den Spieler

        if (controller.isGrounded)
        {
            //Wenn Shift gedrückt, Geschwindigkeit anpassen
            speed = (Input.GetButton("Run")? runSpeed : walkSpeed);
            moveDirection = new Vector3(moveFB, 0, moveLR);
            moveDirection = transform.TransformDirection(moveDirection).normalized;
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

		if (Input.GetKeyDown("escape") || Input.GetKeyDown(KeyCode.Q)) { 
            mouseLocked = !mouseLocked;
            Debug.Log(mouseLocked ? "Mouse locked" : "Mouse unlocked");
        }

        if (mouseLocked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
