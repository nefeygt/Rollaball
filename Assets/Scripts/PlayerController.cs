using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 0;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private int count;
    private Rigidbody rb;

    public InputActionAsset InputActions;

    private InputAction moveAction;
    private Vector2 movementVector;

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();

        moveAction = InputActions.FindAction("Move");
        moveAction.Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    private void Update()
    {
        movementVector = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        RollingMovementWithDamping();
    }

    private void RollingMovementWithDamping()
    {
        Vector3 movementDirection = new Vector3(movementVector.x, 0.0f, movementVector.y);
        rb.AddForce(movementDirection * MoveSpeed, ForceMode.Force);
        Vector3 rollingTorque = Vector3.Cross(Vector3.up, movementDirection);
        rb.AddTorque(rollingTorque * MoveSpeed * 10f, ForceMode.Force);

        if (movementVector.magnitude == 0)
        {
            float dampingFactor = 5f;
            rb.AddForce(-rb.linearVelocity * dampingFactor, ForceMode.Force);

            float angularDampingFactor = 2f;
            rb.AddTorque(-rb.angularVelocity * angularDampingFactor, ForceMode.Force);
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            winTextObject.SetActive(true);

            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;

            SetCountText();
        }
    }
}