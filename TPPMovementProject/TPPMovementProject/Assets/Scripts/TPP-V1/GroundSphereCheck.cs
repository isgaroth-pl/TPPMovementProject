using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSphereCheck : MonoBehaviour
{
    public bool IsGrounded;
    public bool AboutToLand;
    public Transform groundCheckObject;
    private PlayerTPPMovement playerMovement;
    [SerializeField] float groundDistance = 0.05f;
    [SerializeField] float aboutToLandDistance = 0.5f;
    [SerializeField] LayerMask groundMask = default;


    //dodac aby sam tworzyl boiekt ground check jesli nie ma
    private void Awake()
    {
        playerMovement = GetComponent<PlayerTPPMovement>();
    }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheckObject.position, groundDistance, groundMask);
        if (playerMovement.isInTheAir)
        {     
            AboutToLand = Physics.CheckSphere(groundCheckObject.position, aboutToLandDistance, groundMask);
        }
    }
        //debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckObject.position, aboutToLandDistance);
    }
}
