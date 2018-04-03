using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{

    CharacterController player;

    [SerializeField]
    private float speed = 6f;
    [SerializeField]
    private float sensitivity = 2f;
    [SerializeField]
    private GameObject eyes;

    private float moveFB;
    private float moveLR;

    private float rotX;
    private float rotY;
    private float vertVelocity;

    private float jumpDist = 5f;

    void Start()
    {
        player = GetComponent<CharacterController>();
        
    }

    void Update()
    {
        moveFB = Input.GetAxis("Vertical") * speed;
        moveLR = Input.GetAxis("Horizontal") * speed;

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, -89f, 89f);

        Vector3 movement = new Vector3(moveLR, vertVelocity, moveFB);

        transform.Rotate(0, rotX, 0);   //dreht den Player

        eyes.transform.localRotation = Quaternion.Euler(rotY, 0, 0);    //dreht die Kamera

        if (player.isGrounded && Input.GetKeyDown("space"))
        {
            vertVelocity += jumpDist;
        }
        
        vertVelocity = -9.81f * Time.deltaTime;
        player.Move(movement * Time.deltaTime);

        //Debug
        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;
        
        if (player.isGrounded)
        {
            Debug.Log("Jo");
        } else
        {
            Debug.Log("no");
        }
    }
}
