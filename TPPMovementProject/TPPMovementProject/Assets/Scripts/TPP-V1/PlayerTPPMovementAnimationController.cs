using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTPPMovementAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerTPPMovement playerMovement;
    private int _movementStateHash;
    private int _airTimeHash;
    private int _relativeHightHash;
    private int _isInTheAirHash;
    private int _isLaunchingHash;
    private int _isLandingHash;
    private int _jumpStateHash;
    private int _fallTimeHash;
    private int _isAboutToLandHash;


    void Awake()
    {
        CatchComponentsReferences();
        SetStringsToHash();           
    }

    private void Update()
    {
        //360 movement
        animator.SetFloat(_movementStateHash, playerMovement.movementState);
        //launching, jumping, landing and falling
        animator.SetBool(_isLaunchingHash, playerMovement.isLaunching);
        animator.SetBool(_isInTheAirHash, playerMovement.isInTheAir);
        animator.SetBool(_isLandingHash, playerMovement.isLanding);
        animator.SetBool(_isAboutToLandHash, playerMovement.isAboutToLand);
        animator.SetFloat(_airTimeHash, playerMovement.airborneTimer);
        animator.SetFloat(_relativeHightHash, playerMovement.relativeHight);
        animator.SetFloat(_jumpStateHash, playerMovement.jumpState);
        animator.SetFloat(_fallTimeHash, playerMovement.fallTimeAtLanding);
        //others
       
    }

    private void CatchComponentsReferences()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerTPPMovement>();
    }

    private void SetStringsToHash()
    {
        _movementStateHash = Animator.StringToHash("MovementState");
        _airTimeHash = Animator.StringToHash("AirTime");
        _relativeHightHash = Animator.StringToHash("RelativeHight");
        _isInTheAirHash = Animator.StringToHash("IsInTheAir");
        _isLandingHash = Animator.StringToHash("IsLanding");
        _isLaunchingHash = Animator.StringToHash("IsLaunching");
        _jumpStateHash = Animator.StringToHash("JumpState");
        _fallTimeHash = Animator.StringToHash("FallTime");
        _isAboutToLandHash = Animator.StringToHash("IsAboutToLand");
    }

    //public void IsInPivot()
    //{
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //}

}
