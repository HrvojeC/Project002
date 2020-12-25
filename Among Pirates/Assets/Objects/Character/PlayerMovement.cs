using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    private Transform CameraObj = null;
    [SerializeField] private Animator animator = null;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField][Range(0f,1f)] private float interpolation = 0.3f;

    private void Start()
    {
        if (!isLocalPlayer) return;

        CameraObj = GameObject.Find("CameraObj").transform;
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;
        MovePlayerAndCamera();
        Testing();
    }

    private void MovePlayerAndCamera()
    {
        int movem = 0;
        int rotat = 0;

        if (Input.GetKey(KeyCode.W)) movem += 1;
        if (Input.GetKey(KeyCode.S)) movem -= 1;
        if (Input.GetKey(KeyCode.A)) rotat -= 1;
        if (Input.GetKey(KeyCode.D)) rotat += 1;

        transform.Rotate(Vector3.up * rotat * rotationSpeed * Time.deltaTime);
        //controller.Move(Vector3.forward * movem * movementSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * movem * movementSpeed * Time.deltaTime);

        animator.SetBool("Walking", movem != 0);

        CameraObj.position = transform.position;
        CameraObj.eulerAngles = transform.eulerAngles; // Vector3.Lerp(CameraObj.eulerAngles, transform.eulerAngles, interpolation);
    }

    private void Testing ()
    {

    }
}
