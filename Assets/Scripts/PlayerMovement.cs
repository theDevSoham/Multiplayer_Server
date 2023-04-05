using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Player player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;

    private float gravity;
    private float movement;
    private float jumpValue;

    private bool[] inputs;
    private float yVelocity;

    private void OnValidate()
    {
        if(_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        if(player == null)
        {
            player = GetComponent<Player>();
        }

        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        inputs = new bool[6];

        Initialize();
    }

    private void FixedUpdate()
    {
        Vector2 inputDirections = Vector2.zero;

        if (inputs[0])
        {
            inputDirections.y += 1;
        }
        if (inputs[1])
        {
            inputDirections.y -= 1;
        }
        if (inputs[2])
        {
            inputDirections.x -= 1;
        }
        if (inputs[3])
        {
            inputDirections.x += 1;
        }

        Move(inputDirections, inputs[4], inputs[5]);
    }

    private void Move(Vector2 inputDirection, bool jump, bool sprint)
    {
        Vector3 movementDirection = Vector3.Normalize(cameraTransform.right * inputDirection.x + Vector3.Normalize(FlattenVector(cameraTransform.forward * inputDirection.y)));

        movementDirection *= movement;

        if (sprint)
        {
            movementDirection *= 2f;
        }

        if (_controller.isGrounded)
        {
            print("grounded is true");
            yVelocity = 0f;
            if (jump)
            {
                print($"Jump value is: {jump}");
                yVelocity = jumpValue;
            }
        }

        yVelocity += gravity;

        movementDirection.y = yVelocity;
        _controller.Move(movementDirection);

        SendMovement();
    }

    private Vector3 FlattenVector(Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    private void Initialize()
    {
        gravity = gravityMultiplier * Time.fixedDeltaTime * Time.fixedDeltaTime;
        movement = movementSpeed * Time.fixedDeltaTime;
        jumpValue = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void SetInput(bool[] inputs, Vector3 forward)
    {
        this.inputs = inputs;
        cameraTransform.forward = forward;
    }

    private void SendMovement()
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.playerMovement);
        message.AddUShort(player.Id);
        message.AddVector3(cameraTransform.forward);
        message.AddVector3(transform.position);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
