using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//동물을 만들고 방석에 집어넣는다
public class BugFactory : MonoBehaviour
{
    [SerializeField] List<Bug> bugPrefabs = new List<Bug>();
    [SerializeField] List<Bug> madeBugList = new List<Bug>();
    [SerializeField] List<SeatController> _seatControllers = new List<SeatController>();

    public float enterSpeed;
    
    public void MadeBug()
    {
        var targetBug = bugPrefabs[UnityEngine.Random.Range(0, bugPrefabs.Count)];
        var targetSeat = _seatControllers[UnityEngine.Random.Range(0, _seatControllers.Count)];
        var bug = Instantiate(targetBug, this.transform);
        madeBugList.Add(bug);
        bug.EnterToSeat(targetSeat, enterSpeed, 50 + madeBugList.Count);
    }

    public void ClearBugs()
    {
        for (int i = 0; i < madeBugList.Count; i++)
        {
            if (madeBugList[i] != null)
            {
                madeBugList[i].Die();
            }
        }
        madeBugList.Clear();
    }
}
