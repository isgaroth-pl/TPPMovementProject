using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
public class PlayerTPPMovement : MonoBehaviour
{
    public CharacterController controller;
    public GroundSphereCheck groundCheck;
    public PlayerTPPMovementAnimationController animController;
    public Transform cam;   //use main camera - not TPPCamera 

    //Info for animator
    [HideInInspector] public float movementState = 0f;
    [HideInInspector] public float jumpState = 0f;
    public bool isGrounded;
    public bool isAboutToLand;
    public bool isInTheAir = false;
    public bool isLaunching = false;
    public bool isLanding = false;   
    public float airborneTimer = 0f;
    [HideInInspector] public float fallTimeAtLanding;
    [HideInInspector] public float relativeHight = 0f;

    //public bool isTurning180 = false;
    //public float turning180Velocity;
    //movement tweaking
    [SerializeField] float _holdJumpHeight = 5f;
    [SerializeField] float _turnSmoothTime = 0.1f;
    [SerializeField] float _baseJumpHeight = 2f;
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 6f;
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float _movementSlideModifier = 2f;

    private bool _didDoubleJump = false;
    private float _landingTimer = 0f;
    private float _fallTime = 0f;  
    private float _additionalJumpHeight;
    private float _turnSmoothVelocity;
    private Vector3 _currentVelocityReference;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 verticalVelocityVector = Vector3.zero;
    private Vector3 inputVector;
    private Vector3 flatMoveDirection;

    private float _currentMovementSpeed;
    private float _maxMovementSpeed;

    //Input dependent
    private bool isWalkingOn = true;
    private bool isWalkPressed;
    private bool jumpPressed;
    private bool jumpReleased;
    private float horizontal;
    private float vertical;
    
    void Update()
    {
        GetPlayerInputAndState();
        inputVector = new Vector3(horizontal, 0f, vertical);
        SetDesiredMoveDirectionVector(inputVector);             //rotate i set direction 
        RotatePlayerTowardsDesiredMoveDirection(inputVector);   //uwaga na kolejnosc               
        SetMaxMovementSpeed();
        SetCurrentMovementSpeed(inputVector);
        JumpCommandLogic();
        if (verticalVelocityVector != Vector3.zero)
            controller.Move(verticalVelocityVector * Time.deltaTime);
        InTheAirLogic();
        if (!isLaunching)
        {
            controller.Move(Time.deltaTime * moveDirection.normalized * _currentMovementSpeed);
        }
        CalculateStatesForAnimator();
    }


    private void GetPlayerInputAndState()
    {
        //refactor to dependency inversion
        isWalkPressed = Input.GetKeyDown(KeyCode.LeftAlt);
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        jumpReleased = Input.GetKeyUp(KeyCode.Space);
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isGrounded = groundCheck.IsGrounded;
        if (_fallTime > 0f)
            isAboutToLand = groundCheck.AboutToLand;
        else
            isAboutToLand = false;

        if (isWalkPressed && !isInTheAir)
            isWalkingOn = !isWalkingOn;
    }

    private void SetDesiredMoveDirectionVector(Vector3 inputVector)
    {
        if (inputVector.normalized.magnitude >= 0.1f)
        {
            Vector3 direction = inputVector.normalized;
            //rotate player to direction of his movement (and from radians to eulers) + add camera rotation on y axis to rotate at camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //make sure we move to direction of the camera and we * Quaternion with Vector3 to get direction from rotation
            flatMoveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 desiredMoveDirection = flatMoveDirection.normalized;          
            //zniwelowanie naglych zmian kierunku
            //Vector3 desiredMoveDirection = Vector3.SmoothDamp(transform.forward, flatMoveDirection.normalized, ref _currentVelocityReference, 0.5f);
            //Vector3 desiredMoveDirection = Vector3.Slerp(transform.forward, flatMoveDirection.normalized, Time.deltaTime);
            moveDirection = desiredMoveDirection;
         }
    }

    private void RotatePlayerTowardsDesiredMoveDirection(Vector3 inputVector)
    {
        if(inputVector.normalized.magnitude >= 0.1f)
        {
            float turnSmoothTime = _turnSmoothTime;
            if(isInTheAir || isLaunching)
                turnSmoothTime = 4f * _turnSmoothTime;
            if(isLanding)
                turnSmoothTime = 8f * _turnSmoothTime;

            Vector3 direction = inputVector.normalized;
            //rotate player to direction of his movement (and from radians to eulers) + add camera rotation on y axis to rotate at camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //we need to smooth angle (not to snap to positions
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothTargetAngle, 0f);
        }
        else
            return;
    }

    private void SetMaxMovementSpeed()
    {
        if (isWalkingOn)
            _maxMovementSpeed = _walkSpeed;
        if (!isWalkingOn)
            _maxMovementSpeed = _runSpeed;
    }

    private void SetCurrentMovementSpeed(Vector3 inputVector)
    {
        if (_currentMovementSpeed < _maxMovementSpeed && inputVector.magnitude >= 0.1f)
        {
            _currentMovementSpeed += _movementSlideModifier * _maxMovementSpeed * Time.deltaTime;
            if (_currentMovementSpeed > 0.95f * _maxMovementSpeed)
                _currentMovementSpeed = _maxMovementSpeed;
        }
        if (inputVector.magnitude < 0.1f || _currentMovementSpeed > _maxMovementSpeed)
        {
            _currentMovementSpeed -= _movementSlideModifier * _maxMovementSpeed * Time.deltaTime;
            if (_currentMovementSpeed <= 0f)
                _currentMovementSpeed = 0f;
        }
    }

    private void JumpCommandLogic()
    {
        bool canDoubleJump = (jumpPressed && isInTheAir && airborneTimer > 0.2f && _fallTime < 0.1f && !_didDoubleJump);
         //if player wants to jump
        if (jumpPressed && isGrounded && !isLanding)
        {
            verticalVelocityVector.y = 0f;
            _additionalJumpHeight = 0f;
            isLaunching = true;
            _didDoubleJump = false;
        }
        //jump launch plus hold jump logic
        if(isLaunching)
        {
            _additionalJumpHeight += _holdJumpHeight * Time.deltaTime;
            if (_additionalJumpHeight > (_holdJumpHeight - _baseJumpHeight))
            {
                _additionalJumpHeight = (_holdJumpHeight - _baseJumpHeight);
                jumpReleased = true;
            }
            if (jumpReleased)
            {
                verticalVelocityVector.y = Mathf.Sqrt((_baseJumpHeight + _additionalJumpHeight) * -2f * gravityValue);
                isLaunching = false;
                _additionalJumpHeight = 0f;
            }
        }
        //double jump 
        if (canDoubleJump)
        {
            _didDoubleJump = true;
            verticalVelocityVector.y = Mathf.Sqrt(3f * _baseJumpHeight * -2f * gravityValue);
        }
    }

    private void InTheAirLogic()
    {
        //is in the air?
        if ((!isLaunching && verticalVelocityVector.y > 0) || !isGrounded)
        {
            isInTheAir = true;
        }
        //in air
        if (isInTheAir)
        {
            airborneTimer += Time.deltaTime;
            //still in air?
            if (verticalVelocityVector.y <= 0f && isGrounded)
            {
                isInTheAir = false;
            }
            if (verticalVelocityVector.y <= _baseJumpHeight)
            {
                _fallTime += Time.deltaTime;
            }
        }
        //not in air
        if (!isInTheAir)
        {
            if (airborneTimer > 0f)
            {
                //musi byc raz
                isLanding = true;
                fallTimeAtLanding = _fallTime;
                if (fallTimeAtLanding <= 1f)
                    _landingTimer = fallTimeAtLanding;
                else
                    _landingTimer = 1f;
                _fallTime = 0f;
                airborneTimer = 0f;
            }
            //_transformYBeforeFalling = transform.position.y;
            airborneTimer = 0f;
            verticalVelocityVector = Vector3.zero;
        }

        relativeHight = verticalVelocityVector.y;
        //grawitacja
        verticalVelocityVector.y += gravityValue * Time.deltaTime;
        //-2 instead of 0 to force player to ground if groundCheck checks to soon
        if (isGrounded && verticalVelocityVector.y < 0f)
            verticalVelocityVector.y = -2f;

        if (isLanding)
        {
            _landingTimer -= Time.deltaTime;

            //if (fallTimeAtLanding < 0.2f)          
            //    _currentMovementSpeed *= 0.5f;
            //else
                _currentMovementSpeed *= 0.9f;

            if (_landingTimer <= 0)
                isLanding = false;
        }        
    }

    private void CalculateStatesForAnimator()
    {
        //state calculating for aniamtor to use
        movementState = _currentMovementSpeed / _runSpeed;
        jumpState = _additionalJumpHeight / (_holdJumpHeight - _baseJumpHeight);      
    }
}
