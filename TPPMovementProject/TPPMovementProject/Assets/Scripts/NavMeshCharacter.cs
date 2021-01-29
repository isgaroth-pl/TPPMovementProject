using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshCharacter : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    void Awake() => _navMeshAgent = GetComponent<NavMeshAgent>();

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo);

            if(hitSomething)
            {
                Vector3 clickedWorldPoint = hitInfo.point;
                _navMeshAgent.SetDestination(clickedWorldPoint);
            }
        }    
    }
}
