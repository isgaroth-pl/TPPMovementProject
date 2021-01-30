using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookAddOn : MonoBehaviour
{
    [Range(0f, 10f)] public float LookSpeed = 1f;
    public bool InvertY = false;
    private CinemachineFreeLook _freeLookComponent;

    public void Start()
    {
        _freeLookComponent = GetComponent<CinemachineFreeLook>();
    }

    //Update the look movement each time the event is trigger
    public void OnLook(InputAction.CallbackContext context)
    {
        //Normalise a vector an uniform vector in wichever form it came from (gamepad, mouse etc)
        Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
        lookMovement.y = InvertY ? -lookMovement.y : lookMovement.y;

        //X axis only contains between -180 and 180 istead of 0 and 1 like Y axis
        lookMovement.x = lookMovement.x * 180f;

        //adjust axis valuse using look speed and delta time so the look in not dependent of FPS
        _freeLookComponent.m_XAxis.Value += lookMovement.x * LookSpeed * Time.deltaTime;
        _freeLookComponent.m_YAxis.Value += lookMovement.y * LookSpeed * Time.deltaTime;
    }
}
