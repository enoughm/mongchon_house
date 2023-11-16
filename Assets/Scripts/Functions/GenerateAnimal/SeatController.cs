using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using Spine.Unity.Examples;
using SRF;
using SWS;
using UnityEngine;
using Random = UnityEngine.Random;

//동물을 가지고 있다가 밟히면 동물을 내보낸다
public class SeatController : MonoBehaviour
{
    public Camera floorCamera;
    public PathManager EnterPath => enterPath.Random();
    public PathManager ShowingPath => showingTempPaths.Random();
    public int BugCount => bugList.Count;
    public bool CanEnter => bugList.Count < 4;
    
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] SkeletonAnimationHandleExample seatAnimationHandler;
    [SerializeField] List<PathManager> enterPath = new List<PathManager>();
    
    [SerializeField] List<PathManager> showingTempPaths = new List<PathManager>();
    [SerializeField] List<PathManager> exitPaths = new List<PathManager>();
    
    private List<Bug> bugList = new List<Bug>();
    private float bugShowingTime = 0;
    private float bugShowingInterval = 6.5f;

    public bool steped = false;

    public float seatShowingDelay = 0.3f;


    private void Start()
    {
        PlayAnim();
    }

    private void OnEnable()
    {
        Managers.Input.MouseAction += OnMouseEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
    }

    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        
        Debug.Log("OnMouseEvent4");
        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = floorCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Seat");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject.name == gameObject.name)
            {
                Debug.Log("Seat Clicked");
                steped = true;
            }
        }
    }


    public void InsertBug(Bug bug)
    {
        bugList.Add(bug);
        bug.onDie += () => { bugList.Remove(bug); };
    }

    private void Update()
    {
        
        //벌레 일정 간격마다 보여주기
        if (bugList.Count > 0)
        {
            bugShowingTime += Time.deltaTime;
            if (bugShowingTime > bugShowingInterval)
            {
                Debug.Log("Show Bugs");
                bugShowingTime = 0;
                PlayAnim();
                //var path = GetRandomShowingPath();
                //bugList[Random.Range(0, bugList.Count)].ShowingTemp(path, 2f);
            }
        }
        else
        {
            StopAnim();
            bugShowingTime = 0;
        }
        
        //밟으면 벌레 내보내기
        if (steped)
        {
            steped = false;
            if (bugList.Count > 0)
            {
                StopAnim();
                //PlayAnim();
                for (int i = 0; i < bugList.Count; i++)
                {
                    var path = GetRandomExitPath();
                    bugList[i].RunFromSeat(path, 5);
                }
                bugList.Clear();
            }
        }
    }

    private PathManager GetRandomShowingPath()
    {
        int index = UnityEngine.Random.Range(0, showingTempPaths.Count);
        return showingTempPaths[index];
    }

    private PathManager GetRandomExitPath()
    {
        int index = UnityEngine.Random.Range(0, exitPaths.Count);
        return exitPaths[index];
    }

    /// <summary>
    /// 들썩임 애니메이션
    /// </summary>
    public void PlayAnim()
    {
        StopAllCoroutines();
        StartCoroutine(CoPlayAnim());
    }
    
    IEnumerator CoPlayAnim()
    {
        // var st = skeletonAnimation.AnimationName;
        // if (st == 1.ToString())
        //     yield break;
        skeletonAnimation.loop = false;
        skeletonAnimation.timeScale = 1f;
        var anim = seatAnimationHandler.GetAnimationForState("1");
        //var anim = seatAnimationHandler.PlayAnimationForState(1.ToString(), 0);
        seatAnimationHandler.PlayOneShot(anim, 0);
        yield return null;
        //yield return new WaitForSeconds(anim.Duration);
        //skeletonAnimation.ClearState();
    }
    
    // /// <summary>
    // /// 들썩임 애니메이션
    // /// </summary>
    // public void PlayOneAnim()
    // {
    //     StopAllCoroutines();
    //     StartCoroutine(CoPlayAnim());
    // }
    //
    // IEnumerator CoPlayOneAnim()
    // {
    //     var st = skeletonAnimation.AnimationName;
    //     if (st == 1.ToString())
    //         yield break;
    //
    //     skeletonAnimation.loop = false;
    //     var anim =  seatAnimationHandler.PlayAnimationForState(1.ToString(), 0);
    //     yield return 
    // }

    
    
    /// <summary>
    /// To Default Animation
    /// </summary>
    public void StopAnim()
    {
        skeletonAnimation.ClearState();
    }
}
