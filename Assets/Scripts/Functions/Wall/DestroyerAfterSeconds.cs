using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAfterSeconds : MonoBehaviour
{
    public float lifeTime; 

    private void OnEnable()
    {
StopAllCoroutines();
StartCoroutine(CoDestroyAfterSeconds(lifeTime));
    }
    
    IEnumerator CoDestroyAfterSeconds(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Managers.Resource.Destroy(this.gameObject);
    }
}
