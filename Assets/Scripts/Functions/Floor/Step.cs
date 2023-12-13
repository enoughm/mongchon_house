using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Step : MonoBehaviour
{
    private NavMeshObstacle _obstacle;
    private void Awake()
    {
        _obstacle = GetComponent<NavMeshObstacle>();
    }

    private void OnEnable()
    {
        _obstacle.enabled = true;
        //StopAllCoroutines();
        //StartCoroutine(CoDelayObstacle());
    }

    // IEnumerator CoDelayObstacle()
    // {
    //     _obstacle.enabled = false;
    //     yield return new WaitForSeconds(0.5f);
    //     _obstacle.enabled = true;
    // }
    
    
}
