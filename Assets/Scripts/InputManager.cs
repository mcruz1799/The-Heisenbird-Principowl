using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
  public static InputManager S { get; private set; }

  //Associates different keys with actions for the player to take
  private readonly IDictionary<KeyCode, Action> playerControlsMap = new Dictionary<KeyCode, Action>() {
    { KeyCode.UpArrow, Action.Jump },
    { KeyCode.DownArrow, Action.Drop },
    { KeyCode.LeftArrow, Action.MoveLeft },
    { KeyCode.RightArrow, Action.MoveRight },
  };

  //If the player has multiple buttons down, then they're trying to select multiple actions
  //This list represents how to prioritize action selection.
  //An action earlier in the list has a higher priority.
  private readonly IReadOnlyList<Action> playerActionPriorities = new List<Action>() {
    Action.Jump,
    Action.Drop,
    Action.MoveRight,
    Action.MoveLeft,
    Action.Wait,
  };

  //Contains all of the actions the player is trying to do
  private readonly ISet<Action> playerActionSelections = new HashSet<Action>();

  private Action? queuedAction = null;

  private void Awake() {
    S = this;

    //Wait is always considered selected.
    //This is so that if no buttons are being pushed, Wait will automatically be chosen.
    playerActionSelections.Add(Action.Wait);

    if (playerActionPriorities.Count != System.Enum.GetValues(typeof(Action)).Length) {
      Debug.LogError("Player action priorities aren't properly initialized in InputManager!");
      throw new System.InvalidOperationException("Player action priorities aren't properly initialized in InputManager!");
    }
  }

  private void Update() {
    PlayerInputChecks();
  }

  private void PlayerInputChecks() {
    foreach (KeyValuePair<KeyCode, Action> inputMapping in playerControlsMap) {
      KeyCode key = inputMapping.Key;
      Action action = inputMapping.Value;

      if (Input.GetKeyDown(key)) {
        queuedAction = action;
        playerActionSelections.Add(action);
      }

      if (Input.GetKeyUp(key)) {
        playerActionSelections.Remove(action);
      }
    }
  }

  public Action GetPlayerAction() {
    if (queuedAction.HasValue && GameManager.S.Player.CanSelectAction(queuedAction.Value)) {
      Action result = queuedAction.Value;
      queuedAction = null;
      return result;
    }

    foreach (Action action in playerActionPriorities) {
      if (playerActionSelections.Contains(action) && GameManager.S.Player.CanSelectAction(action)) {
        return action;
      }
    }

    Debug.LogError("Player has no legal actions!  Not even Wait!  D:");
    throw new System.InvalidOperationException("Player has no legal actions!  Not even Wait!  D:");
  }
}
