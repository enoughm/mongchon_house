using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//동물을 만들고 방석에 집어넣는다
public class BugFactory : MonoBehaviour
{
    [SerializeField] List<Bug> bugPrefabs = new List<Bug>();
    [SerializeField] List<Bug> madeBugList = new List<Bug>();
    [SerializeField] List<SeatController> _seatControllers = new List<SeatController>();
    
    private int showingBugMin = 4;

    private float time = 0;
    private float interval = 2.5f;
    
    private int bugOrder = 0;
    private int seatOrder = 0;
    
    private void Update()
    {
        if (madeBugList.Count < showingBugMin)
        {
            time += Time.deltaTime;
            if (time > interval)
            {
                time = 0;
                MadeBug();
            }
        }
        else
        {
            time = 0;
        }
    }

    public void MadeBug()
    {
        var targetBugIndex = bugOrder++ % bugPrefabs.Count;
        var targetBug = bugPrefabs[targetBugIndex];
        
            

        var targetSeatIndex = seatOrder++ % _seatControllers.Count;

        if(bugPrefabs.Count == _seatControllers.Count && targetBugIndex == 0)
            targetSeatIndex = seatOrder++ % _seatControllers.Count;

        var targetSeat = _seatControllers[targetSeatIndex];
        var bug = Instantiate(targetBug, this.transform);

        if (!targetSeat.CanEnter || bug == null)
        {
            DestroyImmediate(bug.gameObject);
            return;
        }
        
        madeBugList.Add(bug);
        bug.OnDie += () => madeBugList.Remove(bug);
        bug.EnterToSeat(targetSeat, 50 + madeBugList.Count);
    }
    

    public void ClearBugs()
    {
        for (int i = 0; i < madeBugList.Count; i++)
        {
            if (madeBugList[i] != null)
            {
                madeBugList[i].SetState(Bug.State.Die);
            }
        }
        madeBugList.Clear();
    }
}
