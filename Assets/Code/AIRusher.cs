using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIRusher : MonoBehaviour 
{
    /* Fields */
    public Transform _target;
    NavMeshAgent _navMeshAgent;

	/* Base */
    void Start()
    {
        _target = GameObject.Find("Player").transform;

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _navMeshAgent.destination = _target.position;
    }

    private void Update()
    {
        _navMeshAgent.destination = _target.position;
    }

    /* External */

    /* Internal */
}


