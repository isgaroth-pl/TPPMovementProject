using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController2D : MonoBehaviour
{
    private Animator animator;
    [SerializeField] float _acceleration = 2f;
    [SerializeField] float _decceleration = 2f;
    [SerializeField] float maxWalkVelocity = 0.5f;
    [SerializeField] float maxRunVelocity = 2f;
    private float _velocityZ = 0f;
    private float _velocityX = 0f;


    void Awake() => animator = GetComponent<Animator>();

    private void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool runPressed = Input.GetKey("left shift");

        float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

        if(forwardPressed && _velocityZ < currentMaxVelocity)
            _velocityZ += Time.deltaTime * _acceleration;
        if (leftPressed && _velocityX > -currentMaxVelocity)
            _velocityX -= Time.deltaTime * _acceleration;
        if (rightPressed && _velocityX < currentMaxVelocity)
            _velocityX += Time.deltaTime * _acceleration;
        if(!forwardPressed && _velocityZ > 0f)
            _velocityZ -= Time.deltaTime * _decceleration;    
        if(!forwardPressed && _velocityZ < 0f)
            _velocityZ = 0f;
        if (!leftPressed && _velocityX < 0f)
            _velocityX += Time.deltaTime * _decceleration;
        if (!rightPressed && _velocityX > 0f)
            _velocityX -= Time.deltaTime * _decceleration;
        if (!leftPressed && !rightPressed && _velocityX != 0 && (_velocityX > -0.05f && _velocityX < 0.05f))
            _velocityX = 0f;
        if (forwardPressed && runPressed && _velocityZ > currentMaxVelocity)
            _velocityZ = currentMaxVelocity;
        else if(forwardPressed && _velocityZ > currentMaxVelocity)
        {
            _velocityZ -= Time.deltaTime * _decceleration;
        }

        animator.SetFloat("Velocity Z", _velocityZ);
        animator.SetFloat("Velocity X", _velocityX);
    }


}
