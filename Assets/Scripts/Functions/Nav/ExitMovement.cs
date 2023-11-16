using System;
using System.Collections;
using System.Collections.Generic;
using SRF;
using UnityEngine;
using UnityEngine.AI;

public class ExitMovement : MonoBehaviour
{
    [SerializeField] List<Transform> exitPoints = new List<Transform>();
    
    private NavMeshAgent _agent;
    private Action OnExit;
    private bool setDestination = false;
    private int idx = 0;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void GoToExit(Action onExit)
    {
        OnExit = onExit;
        idx = UnityEngine.Random.Range(0, exitPoints.Count);
        setDestination = true;
    }

    private void Update()
    {
        if (!_agent.enabled)
            return;

        if (setDestination)
        {
            bool ret = _agent.SetDestination(exitPoints[idx].position);
            Debug.Log(ret);
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                Debug.Log("ON EXTI @@@@@@@@@@@@@@@@@@@@");
                OnExit?.Invoke();
                setDestination = false;
            }
        }
    }
}
