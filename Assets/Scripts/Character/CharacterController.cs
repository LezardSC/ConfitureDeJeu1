using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;


[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    public InputReader inputReader;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;
    public GameObject characterModel;
    public CameraMaskHandler cameraMaskHandler;
    private MaskOnlyCollidersHandler maskOnlyCollidersHandler;

    [Tooltip("Layers where the player can stand on")]
    [SerializeField] LayerMask groundMask;
    public float gravityMultiplier;
    public float wallSlideGravityMultiplier;
    private Vector3 globalDown;

    private float axisInput;
    private float lastAxisInput;
    private bool jump;
    private bool mask;
    private bool pause;

    private Quaternion leftRotation = new Quaternion(0, 180, 0, 0);
    private Quaternion rightRotation = new Quaternion(0, 0, 0, 0);

    [Space(10)]
    [Header("Walk Specifics")]
    public float movementSpeed;
    public float dampSpeedUp;
    private bool canMove;

    [Space(10)]
    [Header("Jump Specifics")]
    private bool isGrounded;
    public GameObject groundDetecter;
    public float distToGround;
    public float jumpVelocity;
    private float airbornedTimer;
    [HideInInspector]
    public float afterJumpTimer;
    public float afterJumpTimerLimit;
    private bool startedJump;

    [Space(10)]
    [Header("Jump Specifics")]
    public bool canUseDoubleJump;
    private bool usedDoubleJump;
    public float doubleJumpVelocity;
    private StretchAnimation stretchAnimation;

    [Space(10)]
    [Header("Wall Specifics")]
    public bool canUseWallJump;
    private bool isTouchingWall;
    public float distToWall;
    [Space(5)]
    public float wallJumpVerticalVelocity;
    public float wallJumpHorizontalVelocity;
    private float afterWallJumpTimer;
    public float afterWallJumpTimerLimit;

    [Space(10)]
    [Header("Mask Specifics")]
    public bool canUseMask;
    public Animator maskFilterAnim;
    public Animator maskBackgroundAnim;
    private bool isUsingMask;
    public int afterMaskUseTimer;

    [Space(10)]
    [Header("Pause Specifics")]
    private int afterPauseTimer;
    public GameObject pauseScreen;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        stretchAnimation = GetComponent<StretchAnimation>();
        maskOnlyCollidersHandler = GetComponent<MaskOnlyCollidersHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //axisInput = inputReader.axisInput;
        //jump = inputReader.jump;
        axisInput = Input.GetAxis("Horizontal");
        if (axisInput != 0) lastAxisInput = axisInput;

        if (Input.GetKeyDown("space")) jump = true;
        else jump = false;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            mask = true;
        }
        else mask = false;

        if (Input.GetKeyDown("escape"))
        {
            pause = true;
        }
        else pause = false;

        if (afterPauseTimer > 0 && afterPauseTimer < 20) afterPauseTimer++;
        else afterPauseTimer = 0;

        MoveJump();
        MoveMask();
        MovePause();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckWall();
        RotateCharacterModel();
        MoveWalk();

        if (isGrounded)
        {
            airbornedTimer = 0;
            if (!startedJump) afterJumpTimer = 0;
            afterWallJumpTimer = 0;
            canMove = true;
            usedDoubleJump = false;
        }
        else
        {
            airbornedTimer++;
            if (afterJumpTimer > 0 && afterJumpTimer < afterJumpTimerLimit) afterJumpTimer++;
            else afterJumpTimer = 0;
            if (afterJumpTimer > 4) startedJump = false;

            if (afterWallJumpTimer > 0 && afterWallJumpTimer < afterWallJumpTimerLimit) afterWallJumpTimer++;
            else
            {
                afterWallJumpTimer = 0;
                canMove = true;
            }
        }

        if (afterMaskUseTimer > 0 && afterMaskUseTimer < 25) afterMaskUseTimer++;
        else afterMaskUseTimer = 0;

        ApplyGravity();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(rigidbody.transform.position, -Vector3.up, distToGround + (boxCollider.size.y / 2), groundMask);
    }
    
    private void CheckWall()
    {
        if (Physics.Raycast(rigidbody.transform.position, Vector3.right, distToWall + (boxCollider.size.x / 2), groundMask)
            | Physics.Raycast(rigidbody.transform.position, Vector3.left, distToWall + (boxCollider.size.x / 2), groundMask))
        {
            isTouchingWall = true;
            usedDoubleJump = false;
        }
        else
        {
            isTouchingWall = false;
        }
        /*
        isTouchingWall = (Physics.Raycast(rigidbody.transform.position, new Vector3(0, 0, characterModel.transform.position.z), distToWall + (boxCollider.size.x / 2), groundMask) );
        Debug.DrawLine(rigidbody.transform.position, new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, rigidbody.transform.position.z + 10), Color.red);
        Debug.DrawRay(rigidbody.transform.position, characterModel.transform.position + new Vector3(0, 10, 0));*/
    }

    void RotateCharacterModel()
    {
        if (lastAxisInput < 0) characterModel.transform.rotation = leftRotation;
        else characterModel.transform.rotation = rightRotation;
    }

    void MoveWalk()
    {
        if (canMove) rigidbody.velocity = new Vector3(axisInput * movementSpeed, rigidbody.velocity.y, 0f);
    }

    void MoveJump()
    {
        if (jump && (isGrounded | airbornedTimer < 5) && (afterJumpTimer == 0 | afterJumpTimer > 10))
        {
            rigidbody.velocity = Vector3.up * jumpVelocity;
            startedJump = true;
            afterJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
            afterJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
        }
        else if (jump && canUseWallJump && isTouchingWall && !isGrounded && (afterJumpTimer == 0 | afterJumpTimer > 10) && (afterWallJumpTimer == 0 | afterWallJumpTimer > 10))
        {
            rigidbody.velocity = Vector3.up * wallJumpVerticalVelocity;
            startedJump = true;
            if (Physics.Raycast(rigidbody.transform.position, Vector3.right, boxCollider.size.x / 2 + 0.1f, groundMask))
            {
                rigidbody.AddForce(Vector3.left * wallJumpHorizontalVelocity, ForceMode.Impulse);
            }
            else if (Physics.Raycast(rigidbody.transform.position, Vector3.left, boxCollider.size.x / 2 + 0.1f, groundMask))
            {
                rigidbody.AddForce(Vector3.right * wallJumpHorizontalVelocity, ForceMode.Impulse);
            }
            canMove = false;
            afterWallJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
        }
        else if (jump && !isGrounded && !isTouchingWall && canUseDoubleJump && !usedDoubleJump && (afterJumpTimer == 0 | afterJumpTimer > 10) && afterWallJumpTimer == 0)
        {
            rigidbody.velocity = Vector3.up * doubleJumpVelocity;
            usedDoubleJump = true;
            startedJump = true;
            afterWallJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
        }
    }

    void MoveMask()
    {
        if (mask && canUseMask && afterMaskUseTimer == 0)
        {
            if (isUsingMask)
            {
                maskFilterAnim.CrossFade("MaskFilter_Remove", 0);
                maskBackgroundAnim.CrossFade("MaskBackground_Remove", 0);
                isUsingMask = false;
            }
            else
            {
                maskFilterAnim.CrossFade("MaskFilter_PutOn", 0);
                maskBackgroundAnim.CrossFade("MaskBackground_PutOn", 0);
                isUsingMask = true;
            }
            cameraMaskHandler.ChangeLayerMask(!isUsingMask);
            maskOnlyCollidersHandler.ShowMaskOnlyColliders(isUsingMask);
            afterMaskUseTimer = 1;
        }
    }

    void MovePause()
    {
        if (pause && afterPauseTimer == 0)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pauseScreen.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                pauseScreen.SetActive(true);
            }
        }
    }

    void ApplyGravity()
    {
        Vector3 gravity = Vector3.zero;
        globalDown = Vector3.down.normalized;
        if (isTouchingWall && !isGrounded && (afterJumpTimer == 0 | afterJumpTimer > 30)) gravity = globalDown * wallSlideGravityMultiplier * -Physics.gravity.y;
        else if (startedJump) gravity = globalDown * wallSlideGravityMultiplier * -Physics.gravity.y;
        else gravity = globalDown * gravityMultiplier * -Physics.gravity.y;
        rigidbody.AddForce(gravity);
    }
}
