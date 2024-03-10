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
    private BoxCollider collider;
    public GameObject characterModel;
    public GameObject maskModel;
    public CameraMaskHandler cameraMaskHandler;
    private MaskOnlyCollidersHandler maskOnlyCollidersHandler;
    private NormalOnlyCollidersHandler normalOnlyCollidersHandler;

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

    private Quaternion leftRotation = new Quaternion(0, -180, 0, 0);
    private Quaternion rightRotation = new Quaternion(0, 0, 0, 0);

    [Space(10)]
    [Header("Walk Specifics")]
    public float movementSpeed;
    public float dampSpeedUp;
    private bool canMove;
    public ParticleSystem walkParticleEffect;
    private int walkParticleTimer;

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
    public ParticleSystem jumpParticleEffect;

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
    public ParticleSystem wallParticleEffectLeft;
    public ParticleSystem wallParticleEffectRight;

    [Space(10)]
    [Header("Mask Specifics")]
    public bool canUseMask;
    public Animator maskFilterAnim;
    public Animator maskBackgroundAnim;
    private bool isUsingMask;
    private int afterMaskUseTimer;

    [Space(10)]
    [Header("Pause Specifics")]
    private int afterPauseTimer;
    public GameObject pauseScreen;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        stretchAnimation = GetComponent<StretchAnimation>();
        maskOnlyCollidersHandler = GetComponent<MaskOnlyCollidersHandler>();
        normalOnlyCollidersHandler = GetComponent<NormalOnlyCollidersHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        pauseScreen.SetActive(false);
        maskModel.SetActive(false);
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
            if (axisInput != 0)
            {
                if (walkParticleTimer < 3) walkParticleTimer++;
                else
                {
                    walkParticleTimer = 0;
                    Instantiate<ParticleSystem>(walkParticleEffect, characterModel.transform.position, characterModel.transform.rotation);
                }
            }
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
            if (isTouchingWall && (afterJumpTimer == 0 | afterJumpTimer > 10))
            {
                if (characterModel.transform.rotation == leftRotation) Instantiate<ParticleSystem>(wallParticleEffectLeft, characterModel.transform.position, characterModel.transform.rotation);
                else Instantiate<ParticleSystem>(wallParticleEffectRight, characterModel.transform.position, characterModel.transform.rotation);
            }
        }

        if (afterMaskUseTimer > 0 && afterMaskUseTimer < 25) afterMaskUseTimer++;
        else afterMaskUseTimer = 0;

        ApplyGravity();
    }

    private void CheckGrounded()
    {
        if (Physics.Raycast(rigidbody.transform.position + new Vector3(0.001f, 0 , 0), -Vector3.up, distToGround + (collider.size.y / 2), groundMask)
            | Physics.Raycast(rigidbody.transform.position + new Vector3(-0.001f, 0, 0), -Vector3.up, distToGround + (collider.size.y / 2), groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        //(Physics.Raycast(rigidbody.transform.position, -Vector3.up, distToGround + (collider.size.y / 2), groundMask));
    }

    private void CheckWall()
    {
        if (Physics.Raycast(rigidbody.transform.position, Vector3.right, distToWall + (collider.size.x / 2), groundMask)
            | Physics.Raycast(rigidbody.transform.position, Vector3.left, distToWall + (collider.size.x / 2), groundMask))
        {
            isTouchingWall = true;
            usedDoubleJump = false;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    void RotateCharacterModel()
    {
        if (lastAxisInput < 0) characterModel.transform.rotation = leftRotation;
        else characterModel.transform.rotation = rightRotation;
    }

    void MoveWalk()
    {
        if (canMove)
        {
            rigidbody.velocity = new Vector3(axisInput * movementSpeed, rigidbody.velocity.y, 0f);
        }
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
            Instantiate<ParticleSystem>(jumpParticleEffect, characterModel.transform.position, characterModel.transform.rotation);
        }
        else if (jump && canUseWallJump && isTouchingWall && !isGrounded && (afterJumpTimer == 0 | afterJumpTimer > 10) && (afterWallJumpTimer == 0 | afterWallJumpTimer > 10))
        {
            rigidbody.velocity = Vector3.up * wallJumpVerticalVelocity;
            startedJump = true;
            if (Physics.Raycast(rigidbody.transform.position, Vector3.right, collider.size.x / 2 + 0.1f, groundMask))
            {
                rigidbody.AddForce(Vector3.left * wallJumpHorizontalVelocity, ForceMode.Impulse);
            }
            else if (Physics.Raycast(rigidbody.transform.position, Vector3.left, collider.size.x / 2 + 0.1f, groundMask))
            {
                rigidbody.AddForce(Vector3.right * wallJumpHorizontalVelocity, ForceMode.Impulse);
            }
            canMove = false;
            afterWallJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
            Instantiate<ParticleSystem>(jumpParticleEffect, characterModel.transform.position, characterModel.transform.rotation);
        }
        else if (jump && !isGrounded && !isTouchingWall && canUseDoubleJump && !usedDoubleJump && (afterJumpTimer == 0 | afterJumpTimer > 10) && afterWallJumpTimer == 0)
        {
            rigidbody.velocity = Vector3.up * doubleJumpVelocity;
            usedDoubleJump = true;
            startedJump = true;
            afterWallJumpTimer = 1;
            stretchAnimation.DoStretch("StretchJumpAnimation");
            Instantiate<ParticleSystem>(jumpParticleEffect, characterModel.transform.position, characterModel.transform.rotation);
            afterJumpTimer = 0;
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
            normalOnlyCollidersHandler.ShowMaskOnlyColliders(!isUsingMask);
            maskModel.SetActive(isUsingMask);
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
