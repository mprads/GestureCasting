using Godot;
using System.Collections.Generic;
using Game.States;

namespace Game;

[GlobalClass]
public partial class PlayerStateMachine : Node {

    public enum STATE { IDLE, WALK, SPRINT, CROUCH, JUMP, INAIR }

    [Export]
    private STATE InitialState;

    private Dictionary<STATE, PlayerState> states = new();
    private PlayerState currentState;

    public void Init(Entities.Player player) {
        states[STATE.IDLE] = new PlayerStateIdle();
        states[STATE.WALK] = new PlayerStateWalk();
        states[STATE.SPRINT] = new PlayerStateSprint();
        states[STATE.CROUCH] = new PlayerStateCrouch();
        states[STATE.JUMP] = new PlayerStateJump();
        states[STATE.INAIR] = new PlayerStateInAir();

        foreach (PlayerState state in states.Values) {
            state.player = player;
            state.TransitionRequested += OnTransitionRequested;
        }

        if (InitialState != null) {
            states[InitialState].Enter();
            currentState = states[InitialState];
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (currentState != null) {
            currentState.PhysicsProcess(delta);
        }
    }

    public override void _Input(InputEvent @event) {
        if (currentState != null) {
            currentState.Input(@event);
        }
    }

    private void OnTransitionRequested(PlayerState from, STATE to) {
        if (from != currentState) return;
        if (!states.ContainsKey(to)) return;

        PlayerState nextState = states[to];

        if (currentState != null) {
            currentState.Exit();
        }

        currentState = nextState;
        currentState.Enter();

    }
}
