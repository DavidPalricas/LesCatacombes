using UnityEngine;

/// <summary>
/// The EntityFSM class is responsible for creating a Finite State Machine for the entities in the game.
/// </summary>
public class EntityFSM : MonoBehaviour
{
    /// <summary>
    /// The currentState property represents the current state of the entity.
    /// </summary>
    private IState currentState;

    /// <summary>
    /// The ChangeState method is responsible for changing the current state of the entity.
    /// </summary>
    /// <param name="newState">The new state of the entity.</param>
    public void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;
        currentState.Enter();
    }

    /// <summary>
    /// The Update method is called once per frame (Unity Method).
    /// </summary>
    private void Update()
    {
        currentState?.Execute();
    }
}

/// <summary>
///  The IState interface is responsible for defining the methods that a state must implement.
/// </summary>
public interface IState
{
    /// <summary>
    /// The Enter method is responsible for executing the logic when the state is entered.
    /// </summary>
    void Enter();

    /// <summary>
    /// The Execute method is responsible for executing the logic of the state.
    /// </summary>
    void Execute();

    /// <summary>
    /// The Exit method is responsible for executing the logic when the state is exited.
    /// </summary>
    void Exit();
}

/// <summary>
/// The EntityStateBase class is responsible for defining the base class for the states of the entity.
/// </summary>
public abstract class EntityStateBase : IState
{
    /// <summary>
    /// The entityFSM property represents the Finite State Machine of the entity.
    /// </summary>
    protected EntityFSM entityFSM;

    /// <summary>
    /// Creates a new instance of the <see cref="EntityStateBase"/> class.
    /// </summary>
    /// <param name="entityFSM">The entity's finite state machine.</param>
    public EntityStateBase(EntityFSM entityFSM)
    {
        this.entityFSM = entityFSM;
    }

    /// <summary>
    /// The Enter method is responsible for executing the logic when the state is entered.
    /// It is a virtual method that can be overridden by the derived classes.
    /// It is inirrated from the IState interface.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// The Execute method is responsible for executing the logic of the state.
    /// It is a virtual method that can be overridden by the derived classes.
    /// It is inirrated from the IState interface.
    /// </summary>
    public virtual void Execute() { }

    /// <summary>
    /// The Exit method is responsible for executing the logic when the state is exited.
    /// It is a virtual method that can be overridden by the derived classes.
    /// It is inirrated from the IState interface.
    /// </summary>
    public virtual void Exit() { }
}