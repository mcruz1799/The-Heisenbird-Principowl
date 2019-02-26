using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  public static GameManager S { get; private set; }

#pragma warning disable 0649
  [Range(0.01f, 2f)] [SerializeField] private float timeBetweenTurns = 0.01f;
  [SerializeField] private Player _player;
  [SerializeField] private Board _board;
#pragma warning restore 0649

  public Player Player => _player;
  public Board Board => _board;

  //This is public so that other objects can calculate how long their animations should take.
  public float TimeBetweenTurns => timeBetweenTurns;

  //Initialized here so that other classes can safely call RegisterTurnTaker inside of Awake
  private ISet<ITurnTaker> turnTakers = new HashSet<ITurnTaker>();

  private void Awake() {
    S = this;

    StartCoroutine(TurnTakerRoutine());
  }

  public bool RegisterTurnTaker(ITurnTaker turnTaker) {
    return turnTakers.Add(turnTaker);
  }
  public bool UnregisterTurnTaker(ITurnTaker turnTaker) {
    return turnTakers.Remove(turnTaker);
  }

  private IEnumerator TurnTakerRoutine() {
    while (true) {
      yield return new WaitForSeconds(timeBetweenTurns);

      Player.SelectAction(InputManager.S.GetPlayerAction());
      foreach (ITurnTaker turnTaker in turnTakers) {
        turnTaker.OnTurn();
      }
    }
  }
}
