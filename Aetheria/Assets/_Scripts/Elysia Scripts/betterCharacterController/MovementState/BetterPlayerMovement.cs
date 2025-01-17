using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody))]
public class BetterPlayerMovement : MonoBehaviour
{

    #region Movement
    [HideInInspector] public float currentMoveSpeed;
    public float walkSpeed =10, walkBackSpeed=5;
    public float runSpeed = 20, runBackSpeed = 15;
    public float crouchSpeed =5, crouchBackSpeed =3;
    public float airSpeed = 1.5f;
   
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput;
    CharacterController controller;

    //public Rigidbody rb;
    //public bool canDoubleJump;
    #endregion

    #region GroundCheck
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    #endregion

    #region Gravity
    [SerializeField] float gravity = -30f;
    [SerializeField] float jumpForce = 15;
    [HideInInspector] public bool jumped;
    Vector3 velocity;
    #endregion

    #region States
    public MovementBaseState previousState;
    public MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    #endregion

    [HideInInspector] public Animator anim;


    //private void Awake()
    // {
    //     rb = GetComponent<Rigidbody>();
   // }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(Idle);

    }

    // Update is called once per frame
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();

        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (IsGrounded())
        //    {
        //        rb.velocity = Vector3.up * jumpForce;
        //        canDoubleJump = true;
        //    }
        //    else if (canDoubleJump)
        //    {
        //        rb.velocity = Vector3.up * jumpForce;
        //        canDoubleJump = false;
        //    }
        //}

    }

    public void SwitchState(MovementBaseState state) 
    {

        currentState = state;
        currentState.EnterState(this);
    
    }

    void GetDirectionAndMove() {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero;
        if (!IsGrounded()) airDir = transform.forward * vInput + transform.right * hzInput;
        else dir = transform.forward * vInput + transform.right * hzInput;

        controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime);


    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
        return false;
    }

    void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);

    }

    void Falling() => anim.SetBool("Falling", !IsGrounded());

    public void JumpForce()
    {
        
        

        velocity.y = jumpForce;

    }

    public void Jumped() => jumped = true;
}
