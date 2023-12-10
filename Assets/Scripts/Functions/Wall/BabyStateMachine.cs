using UnityEngine;
using KevinCastejon.FiniteStateMachine;
using Sirenix.OdinInspector;

public class BabyStateMachine : AbstractFiniteStateMachine
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float stopDistance = 0;
    [SerializeField] private bool reached;

    [SerializeField] private LightManager lightManager;
    [SerializeField] private BabyAnimStateMachine animStateMachine;
    [SerializeField] private CandleController leftCandle;
    [SerializeField] private CandleController rightCandle;
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
        TransitionToState(State.LIGHTONLEFT);
    }
    
    [Button]
    public void TurnOffLight()
    {
        TransitionToState(State.TO_INIT);
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

        if (Vector3.Distance(transform.position, moveTargetTr.position) < stopDistance)
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
        private bool done;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.animStateMachine.ToWalk();
            _machine.SetMoveTarget(_machine.leftCandle.transform);
            done = false;
        }
        public override void OnUpdate()
        {
            if (_machine.reached && !done)
            {
                done = true;
                _machine.animStateMachine.ToLightOn();
                _machine.leftCandle.LightOn(1f,() =>
                {
                    TransitionToState(State.LIGHTONRIGHT);
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
        private bool done;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.animStateMachine.ToWalk();
            _machine.SetMoveTarget(_machine.rightCandle.transform);
            done = false;
        }
        public override void OnUpdate()
        {
            if (_machine.reached && !done)
            {
                done = true;
                _machine.animStateMachine.ToLightOn();
                _machine.rightCandle.LightOn(1f, () =>
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
    
    public class FollowRunState : AbstractState
    {
        private BabyStateMachine _machine;
        public override void OnEnter()
        {
            _machine = GetStateMachine<BabyStateMachine>();
            _machine.animStateMachine.ToIdle();
        }
        
        public override void OnUpdate()
        {
            //check player of front
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
            _machine.animStateMachine.ToWalk();
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
