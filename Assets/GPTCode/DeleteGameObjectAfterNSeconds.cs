using System.Collections;
using UnityEngine;

public class DeleteGameObjectAfterNSeconds : MonoBehaviour
{
    [Tooltip("The number of seconds after which the GameObject should be deleted.")]
    public float deleteAfterSeconds = 5f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(deleteAfterSeconds);
        Managers.Resource.Destroy(gameObject);
    }
}