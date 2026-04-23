using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("基础属性")]
    public Rigidbody2D rb;
    public float speed;
    public float jumpForce;

    [Header("基础属性")]
    public float health;
    public bool isDead = false;
    public PlayerAnimation playerAnimation;

    [Header("地面检测")]
    public Transform groundCheck;//检测中心
    public float checkRadius;//检测半径
    public LayerMask groundLayer;//对象图层
    public bool isGround;//是否在地面

    public PhysicsCheck physicsCheck;



    [Header("特效")]
    public GameObject jumpFX;
    public GameObject landFX;

    [Header("炸弹")]
    public GameObject bombPrefab;
    public float nextAttack = 0;//攻击冷却
    public float attackRate;//攻击频率

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();

        //RegisterHandle();//登录手柄


        GameManager.instance.IsPlayer(this);


        health = GameManager.instance.LoadHealth();
        UIManager.instance.UpdateHealth(health);
    }






    public void Update()
    {
        //playerAnimation.anim.SetBool("dead", isDead);
        //if (isDead){ return; }
        //
        //CheckInput();





        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();


    }//输入用Update（听）

    public void FixedUpdate()
    {
        if (isDead)
        { 
            rb.velocity = Vector2.zero;
            return;
        }//死亡后不能滑行


        //PhysicsCheck();
        //Movement();
        //_Jump();




        rb.velocity = new Vector2(inputDirection.x * speed , rb.velocity.y);
        //翻转
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 1;
            //transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (inputDirection.x < 0)
        {
            faceDir = -1;
            //transform.eulerAngles = new Vector3(0, 180, 0);
        }
        transform.localScale = new Vector3(faceDir, 1, 1);



    }//每帧执行动作用FixedUpdate（做）







    float inputX = 0f;//兼有【摇杆joyX】【手柄input.x】【键盘kx】【旧输入系统Horizontal】多种渠道的水平输入
    float inputY = 0f;//兼有【摇杆joyY】【手柄input.y】【键盘ky】【旧输入系统Vertical】多种渠道的水平输入

    void Movement()
    {
        // 1. 先读摇杆
        float joyX = Joystick.Horizontal;
        float joyY = Joystick.Vertical;

        if (Mathf.Abs(joyX) > 0.01f || Mathf.Abs(joyY) > 0.01f)
        {
            inputX = joyX;
            inputY = joyY;
        }
        else
        {
            // 2. 再读 Input System
            Vector2 input = moveAction.action.ReadValue<Vector2>();

            if (input.sqrMagnitude > 0.001f)
            {
                inputX = input.x;
                inputY = input.y;
            }
            else
            {
                // 3. 再读 Keyboard.current
                float kx = 0f;
                float ky = 0f;

                if (Keyboard.current != null)
                {
                    if (Keyboard.current.wKey.isPressed) ky += 1;
                    if (Keyboard.current.sKey.isPressed) ky -= 1;
                    if (Keyboard.current.dKey.isPressed) kx += 1;
                    if (Keyboard.current.aKey.isPressed) kx -= 1;
                }

                if (Mathf.Abs(kx) > 0.01f || Mathf.Abs(ky) > 0.01f)
                {
                    inputX = kx;
                    inputY = ky;
                }
                else
                {
                    // 4. 最后兜底旧输入
                    inputX = Input.GetAxisRaw("Horizontal");
                    inputY = Input.GetAxisRaw("Vertical");
                }
            }
        }


        // 最终统一使用 inputX / inputY
        float horizontalInput = inputX;



        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
        }

        //if (horizontalInput > 0)
        //{
        //    transform.eulerAngles = new Vector3(0, 0, 0);
        //}
        //if (horizontalInput < 0)
        //{
        //    transform.eulerAngles = new Vector3(0, 180, 0);
        //}
    }



    [Header("跳跃相关")]
    private bool canJump;//是否按下了按钮
    public bool isJump;//是否位于跳跃中

   // void _Jump()
   // {
   //     if (canJump)
   //     {
   //         isJump = true;
   //         rb.velocity = new Vector2(rb.velocity.x, jumpForce);
   //         canJump = false; // 提取文字中未显示，但逻辑上建议加上，防止无限跳跃
   //
   //         jumpFX.SetActive(true);
   //         jumpFX.transform.position = transform.position+new Vector3(0,-0.45f,0);
   //     }
   // }//持续跳跃
   // void PhysicsCheck() 
   // {
   //     isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
   // 
   //     if (isGround) { isJump = false; }//跳跃结束
   // 
   // }//地面检测
    public void LandFX()//动画帧时间触发
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }


   //public void OnDrawGizmos()
   //{
   //    Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
   //}//显示检测圆环(不需要调用)



    public void Attack()
    {
        if (Time.time > nextAttack)
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);

            nextAttack = Time.time + attackRate;
        }
    }






    //void CheckInput()
    //{
    //    if (Keyboard.current.spaceKey.isPressed && isGround)
    //    {
    //        canJump = true;
    //    }
    //
    //    if (Keyboard.current.jKey.isPressed && isGround)
    //    {
    //        Attack();
    //    }
    //}

    public void GetHit(float damage)
    {
        if (!playerAnimation.anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit"))
        {
            health -= damage;

            if (health < 1)
            {
                health = 0;
                isDead = true;
            }
            playerAnimation.anim.SetTrigger("hit");

            UIManager.instance.UpdateHealth(health);

        }//在Hit动画状态中，不会受伤


    
    }

    /// <summary>
    /// 多端输入
    /// </summary>
    #region
    [Header("多端输入InputSystem")]
    [SerializeField] private InputActionReference moveAction;//方向键控制
    [SerializeField] private InputActionAsset inputActions;//跑攻闪

    private InputAction runAction;

    private InputAction AttackAction;

    private InputAction DodgeAction;

    private InputAction InteractAction;

    private InputAction MenuAction;

    public bool isInputBlocked = true;//在捏人界面暂时切断玩家的输入

    private void RegisterHandle()
    {
        // 获取动作（根据你的Action Map结构可能需要调整路径）
        runAction = inputActions.FindAction("Run");
        AttackAction = inputActions.FindAction("Attack");
        DodgeAction = inputActions.FindAction("Dodge");
        InteractAction = inputActions.FindAction("Interact");
        MenuAction = inputActions.FindAction("Menu");


        // 订阅输入事件
        runAction.started += OnRunStarted;
        runAction.canceled += OnRunCanceled;

        // 订阅输入事件
        AttackAction.started += OnAttackStarted;
        AttackAction.canceled += OnAttackCanceled;

        // 订阅输入事件
        DodgeAction.started += OnDodgeStarted;
        DodgeAction.canceled += OnDodgeCanceled;

        // 订阅输入事件
        InteractAction.started += OnInteractStarted;
        InteractAction.canceled += OnInteractCanceled;

        // 订阅输入事件
        MenuAction.started += OnMenuStarted;
        MenuAction.canceled += OnMenuCanceled;
    }
    private void OnRunStarted(InputAction.CallbackContext context)
    {

        isRunning = true;

    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {

        isRunning = false;
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {

        

    }
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {

       
    }

    private void OnDodgeStarted(InputAction.CallbackContext context)
    {

        canJump = true;

    }
    private void OnDodgeCanceled(InputAction.CallbackContext context)
    {
        canJump = false;

    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {

        isInteracting = true;

    }
    private void OnInteractCanceled(InputAction.CallbackContext context)
    {

        isInteracting = false;
    }
    private void OnMenuStarted(InputAction.CallbackContext context)
    {



    }
    private void OnMenuCanceled(InputAction.CallbackContext context)
    {



    }

    [Header("手机端触发")]
    public Joystick Joystick;

    //手机端触发
    public bool isRunning = false;//持续按下跑步键
    public void ButtonSetRun()
    {

        isRunning = true;

    }
    public void ButtonSetStop()
    {
        isRunning = false;

    }

    //手机端触发
    public bool isAttacking = false;//持续按下攻击键
    public void ButtonSetAttack()
    {

    }
    public void ButtonSetAttackOver()
    {


    }

    //手机端触发
    public bool isDodging = false;//持续按下闪避键
    public void ButtonSetDodge()
    {

    }
    public void ButtonSetDodgeOver()
    {

    }


    //手机端触发
    public bool isInteracting = false;//持续按下交互键
    public GameObject InteractingButton;
    public void ButtonSetInteract()
    {

        isInteracting = true;
    }
    public void ButtonSetInteractOver()
    {
        isInteracting = false;
    }



    //手机端触发
    //public bool isMenu = false;//持续按下交互键
    //public void ButtonSetMenu()
    //{
    //
    //    if (!isDie && currentHealth > 0 && !isInputBlocked && IsGrounded())
    //    {
    //        //isMenu = true;
    //
    //        UIManager.instance.OpenCloseMenu();
    //        AudioManager.instance.AudioPlay(AudioManager.instance.Bullet_AK);
    //    }
    //}
    //public void ButtonSetMenuOver()
    //{
    //
    //    if (!isDie && currentHealth > 0 && !isInputBlocked && IsGrounded())
    //    {
    //        //isMenu = false;
    //    }
    //}
    #endregion


    /// <summary>
    /// 多端输入2
    /// </summary>
    #region
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private void Awake()
    {
        inputControl = new PlayerInputControl();

        inputControl.Gameplay.Jump.started += Jump;

    }

    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }


    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround) 
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        
    }

    #endregion
}
