using System.Collections;
using System.Collections.Generic;
using MelenitasDev.SoundsGood;
using Spine.Unity;
using Spine.Unity.Examples;
using SRF;
using SWS;
using UnityEngine;

//동물을 가지고 있다가 밟히면 동물을 내보낸다
public class SeatController : MonoBehaviour
{
    public PathManager EnterPath => enterPath.Random();
    public int BugCount => bugList.Count;
    public bool CanEnter => bugList.Count < 2;
    
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] SkeletonAnimationHandleExample seatAnimationHandler;
    [SerializeField] List<PathManager> enterPath = new List<PathManager>();
    
    [SerializeField] List<PathManager> showingTempPaths = new List<PathManager>();
    [SerializeField] List<PathManager> exitPaths = new List<PathManager>();
    
    private List<Bug> bugList = new List<Bug>();
    private float bugShowingTime = 0;
    private float bugShowingInterval = 6.5f;
    private int prevBugCount = 0;

    public bool steped = false;

    public float seatShowingDelay = 0.3f;
    private Camera _targetCamera;

    private void Awake()
    {
        _targetCamera = GameObject.FindGameObjectWithTag("FloorCam").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Game.FloorUrgTouchDetector.HokuyoAction += OnHokuyoEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Game.FloorUrgTouchDetector.HokuyoAction -= OnHokuyoEvent;
    }

    public void InsertBug(Bug bug)
    {
        bugList.Add(bug);
        bug.OnDie += () => { bugList.Remove(bug); };
    }

    private void Update()
    {
        
        //벌레 일정 간격마다 보여주기
        if (bugList.Count > 0)
        {
            bugShowingTime += Time.deltaTime;
            if (bugShowingTime > bugShowingInterval)
            {
                bugShowingTime = 0;
                PlayAnim();
            }

            //벌레 수가 달라졌을때 흔들림
            if (prevBugCount != bugList.Count)
                PlayAnim();
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
                    bugList[i].SetState(Bug.State.OutSeat);
                }
                bugList.Clear();
            }
        }
        
        
        prevBugCount = bugList.Count;
    }

    private PathManager GetRandomShowingPath()
    {
        int index = UnityEngine.Random.Range(0, showingTempPaths.Count);
        return showingTempPaths[index];
    }

    public PathManager GetRandomExitPath()
    {
        int index = UnityEngine.Random.Range(0, exitPaths.Count);
        return exitPaths[index];
    }

    /// <summary>
    /// 들썩임 애니메이션
    /// </summary>
    private void PlayAnim()
    {
        StopAllCoroutines();
        StartCoroutine(CoPlayAnim());
    }
    
    IEnumerator CoPlayAnim()
    {
        skeletonAnimation.loop = false;
        skeletonAnimation.timeScale = 1f;
        var anim = seatAnimationHandler.GetAnimationForState("1");
        skeletonAnimation.AnimationState.SetAnimation(0, anim, false);
        yield return null;
    }
    
    /// <summary>
    /// To Default Animation
    /// </summary>
    public void StopAnim()
    {
        skeletonAnimation.ClearState();
    }
    
    
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = _targetCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Seat");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject == gameObject)
            {
                Debug.Log("Seat Clicked");
                steped = true;
            }
        }
    }
    
    private void OnHokuyoEvent(UrgTouchState state, Vector2 arg2)
    {
        switch (state)
        {
            case UrgTouchState.Empty:
                break;
            case UrgTouchState.TouchMoment:
                break;
            case UrgTouchState.TouchDown:
                break;
            case UrgTouchState.TouchPress:
                Ray ray = Managers.Game.FloorCamera.ViewportPointToRay(arg2);
                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer("Seat");
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (hit.transform.gameObject.Equals(gameObject))
                    {
                        //isStep = true;
                        steped = true;
                    }
                }
                break;
            case UrgTouchState.TouchPressUp:
                break;
            case UrgTouchState.TouchClicked:
                break;
        }
    }
}
