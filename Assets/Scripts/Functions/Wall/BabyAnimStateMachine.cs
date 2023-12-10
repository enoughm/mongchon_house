using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using KevinCastejon.FiniteStateMachine;
using Sirenix.OdinInspector;
using Spine.Unity;
using Spine.Unity.Examples;

public class BabyAnimStateMachine : AbstractFiniteStateMachine
{
    public enum State
    {
        IDLE,
        INTERACTION_ONE,
        INTERACTION_TWO,
        WALK,
        RUN,
        LIGHT_ON
    }
    
    public SkeletonAnimationHandleExample idleObject;
    public SkeletonAnimationHandleExample interactionObject;
    public SkeletonAnimationHandleExample walkAndOnObject;
    
    private void Awake()
    {
        Init(State.IDLE,
            AbstractState.Create<IdleState, State>(State.IDLE, this),
            AbstractState.Create<InteractionOneState, State>(State.INTERACTION_ONE, this),
            AbstractState.Create<InteractionTwoState, State>(State.INTERACTION_TWO, this),
            AbstractState.Create<WalkState, State>(State.WALK, this),
            AbstractState.Create<RunState, State>(State.RUN, this),
            AbstractState.Create<LightOnState, State>(State.LIGHT_ON, this)
        );
    }

    [Button]public void ToIdle() => TransitionToState(State.IDLE);
    [Button]public void ToInteractionOne()=> TransitionToState(State.INTERACTION_ONE);
    [Button]public void ToInteractionTwo()=> TransitionToState(State.INTERACTION_TWO);
    [Button]public void ToWalk() =>  TransitionToState(State.WALK);
    [Button]public void ToRun() => TransitionToState(State.RUN);
    [Button]public void ToLightOn() => TransitionToState(State.LIGHT_ON);

    private SkeletonAnimationHandleExample UpdateObjects(State state)
    {
        idleObject.gameObject.SetActive(state == State.IDLE);
        interactionObject.gameObject.SetActive(state is State.INTERACTION_ONE or State.INTERACTION_TWO);
        walkAndOnObject.gameObject.SetActive(state is State.LIGHT_ON or State.WALK or State.RUN);

        switch (state)
        {
            case State.IDLE:
                return idleObject;
            case State.INTERACTION_ONE:
            case State.INTERACTION_TWO:
                return interactionObject;
            case State.WALK:
            case State.RUN:
            case State.LIGHT_ON:
                return walkAndOnObject;
        }

        return idleObject;
    }
    
    
    
    public class IdleState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.skeletonAnimation.loop = true;
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class InteractionOneState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.PlayAnimationForState("interaction", 0);
            curAnimationHandler.skeletonAnimation.loop = true;
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class InteractionTwoState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.skeletonAnimation.loop = false;
            CoAnim();
        }

        async void CoAnim()
        {
            var animation = curAnimationHandler.PlayAnimationForState("interaction2", 0);
            await Task.Delay(TimeSpan.FromSeconds(animation.Duration));
            TransitionToState(State.INTERACTION_ONE);
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class WalkState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.skeletonAnimation.loop = true;
            Debug.Log(curAnimationHandler.gameObject.name);
            var animation = curAnimationHandler.PlayAnimationForState("walk", 0);
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class RunState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.skeletonAnimation.loop = true;
            Debug.Log(curAnimationHandler.gameObject.name);
            var animation = curAnimationHandler.PlayAnimationForState("run", 0);
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class LightOnState : AbstractState
    {
        private SkeletonAnimationHandleExample curAnimationHandler;
        public override void OnEnter()
        {
            curAnimationHandler = GetStateMachine<BabyAnimStateMachine>().UpdateObjects(GetEnumValue<State>());
            curAnimationHandler.skeletonAnimation.loop = false;
            CoAnim();
        }
        
        async void CoAnim()
        {
            var animation = curAnimationHandler.PlayAnimationForState("on", 0);
            curAnimationHandler.skeletonAnimation.loop = false;
        }
        
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
}
