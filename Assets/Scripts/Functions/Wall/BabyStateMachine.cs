using UnityEngine;
using KevinCastejon.FiniteStateMachine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class BabyStateMachine : AbstractFiniteStateMachine
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float stopDistance = 0;
    [SerializeField] private bool reached;

    [SerializeField] private LightManager lightManager;
    [SerializeField] private BabyAnimStateMachine animStateMachine;
    [SerializeField] private CandleController leftCandle;
    [SerializeField] private CandleController rightCandle;
    [FormerlySerializedAs("rightCandleFirePos")] [SerializeField] private Transform rightCandleFireTr;
    [SerializeField] private Transform initTr;
    [SerializeField] private Transform moveTargetTr;


    
    public enum State
    {
        IDLE,
        SIT,
        LIGHTONLEFT,
        LIGHTONRIGHT,
        FollowRun,
        TO_INIT
    }
    private void Awake()
    {
        Init(State.IDLE,
            AbstractState.Create<IdleState, State>(State.IDLE, this),
            AbstractState.Create<SitState, State>(State.SIT, this),
            AbstractState.Create<LightOnLeftState, State>(State.LIGHTONLEFT, this),
            AbstractState.Create<LightOnRightState, State>(State.LIGHTONRIGHT, this),
            AbstractState.Create<FollowRunState, State>(State.FollowRun, this),
            AbstractState.Create<ToInitState, State>(State.TO_INIT, this)
        );
    }

    protected override bool OnAnyStateUpdate()
    {
        SetDirection();
        MoveToTargetTr();
        return base.OnAnyStateUpdate();
    }
    
    [Button]
    public void TurnOnLight()
    {
        TransitionToState(State.LIGHTONRIGHT);
    }
    
    [Button]
    public void TurnOffLight()
    {
        TransitionToState(State.TO_INIT);
    }

    private void SetSpeed(bool run)
    {
        if(run)
            animStateMachine.ToRun();
        else
            animStateMachine.ToWalk();
        
        speed = run ? 6 : 3;
    }

    private void SetMoveTarget(Transform transform)
    {
        moveTargetTr = transform;
    }

    private void SetDirection()
    {
        if (moveTargetTr != null)
        {
            bool isLeft = moveTargetTr.position.x < transform.position.x;
            if(isLeft)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void MoveToTargetTr()
    {
        if (moveTargetTr == null)
            return;

        if (Vector3.Distance(transform.position , moveTargetTr.position) < stopDistance )
        {
            reached = true;
            return;
        }
        else
        {
            reached = false;
        }
        
        //vector move
        Vector3 dir = moveTargetTr.position - transform.position;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
    }
    
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;
			
        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = Managers.Game.WallCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Baby");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject == gameObject)
            {
                //anagers.Sound.PlaySfx(SFX.CharacterClick);
                //StateChange();
            }
        }
    }
    

    public class IdleState : AbstractState
    {
        private float _time = 0;
        private float _maxTime = 3;
        private bool _init = false;
        public override void OnEnter()
        {
            if(!_init)
                GetStateMachine<BabyStateMachine>().animStateMachine.ToInteractionOne();
            else
                GetStateMachine<BabyStateMachine>().animStateMachine.ToIdle();
            _time = 0;
        }
        public override void OnUpdate()
        {
            _time += Time.deltaTime;
            if (_time > _maxTime)
            {
                TransitionToState(State.SIT);
            }
        }
        public override void OnExit()
        {
        }
    }
    public class SitState : AbstractState
    {
        public override void OnEnter()
        {
            GetStateMachine<BabyStateMachine>().animStateMachine.ToInteractionOne();
        }
        
        public override void OnUpdate()
        {
            
        }
        
        public override void OnExit()
        {
        }
    }

    public class LightOnLeftState : AbstractState
    {
        private BabyStateMachine _machine;
        private bool done1;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.SetMoveTarget(_machine.leftCandle.transform);
            _machine.SetSpeed(true);
            _machine.stopDistance = 1.35f;
            done1 = false;
        }
        public override void OnUpdate()
        {
            if (_machine.reached && !done1)
            {
                done1 = true;
                _machine.animStateMachine.ToLightOn();
                Managers.Game.SendPacketLightOnLeft();
                _machine.leftCandle.LightOn(1f,() =>
                {
                    TransitionToState(State.FollowRun);
                    _machine.lightManager.ToLight(3);
                });
            }
        }
        public override void OnExit()
        {
        }
    }
    
    public class LightOnRightState : AbstractState
    {
        private BabyStateMachine _machine;
        private bool done1;
        private bool done2;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.SetMoveTarget(_machine.rightCandleFireTr);
            _machine.SetSpeed(false);
            _machine.stopDistance = 0.05f;
            done1 = false;
            done2 = false;
        }
        public override void OnUpdate()
        {
            if (_machine.reached && !done1)
            {
                done1 = true;
                _machine.SetSpeed(true);
                _machine.SetMoveTarget(_machine.rightCandle.transform);
                _machine.stopDistance = 1.35f;
            }
            else if (_machine.reached && !done2)
            {
                done2 = true;
                _machine.animStateMachine.ToLightOn();
                Managers.Game.SendPacketLightOnRight();
                _machine.rightCandle.LightOn(1f, () =>
                {
                    TransitionToState(State.LIGHTONLEFT);
                });
            }
        }
        public override void OnExit()
        {
        }
    }
    
    public class FollowRunState : AbstractState
    {
        private BabyStateMachine _machine;
        private bool done;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.SetSpeed(false);
            _machine.SetMoveTarget(_machine.initTr);
            _machine.stopDistance = 0.05f;
            done = false;
        }
        
        public override void OnUpdate()
        {
            //check player of front
            if (_machine.reached && !done)
            {
                done = true;
                TransitionToState(State.IDLE);
            }
        }
        
        public override void OnExit()
        {
        }
    }
    public class ToInitState : AbstractState
    {
        private BabyStateMachine _machine;
        private bool done;
        
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.SetSpeed(false);
            _machine.SetMoveTarget(_machine.initTr);
            _machine.lightManager.ToDark(3);
            done = false;
        }
        public override void OnUpdate()
        {
            if (_machine.reached && !done)
            {
                done = true;
                TransitionToState(State.IDLE);
            }
        }
        public override void OnExit()
        {
        }
    }
}
