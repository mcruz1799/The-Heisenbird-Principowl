using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  public static GameManager S { get; private set; }

#pragma warning disable 0649
  [Range(0.01f, 2f)] [SerializeField] private float timeBetweenTurns = 0.01f;
  [SerializeField] private PlayerObject _playerObject;
  [SerializeField] private BoardObject boardMaker;
#pragma warning restore 0649

  public Player Player { get; private set; }
  public Board Board { get; private set; }
  public Transform TileInhabitantObjectHolder { get; private set; }

  //This is public so that other objects can calculate how long their animations should take.
  public float TimeBetweenTurns => timeBetweenTurns;

  //Initialized here so that other classes can safely call RegisterTurnTaker inside of Awake
  private ISet<ITurnTaker> turnTakers = new HashSet<ITurnTaker>();

  private void Awake() {
    S = this;
    TileInhabitantObjectHolder = new GameObject().transform;
    TileInhabitantObjectHolder.name = "TileInhabitantObjectHolder";
    Board = new Board(boardMaker.numRows, boardMaker.numCols, boardMaker.tilePrefab, boardMaker.transform);
    boardMaker.PopulateBoard();

    Player = new Player(_playerObject);
    if (Board == null || Player == null) {
      throw new System.Exception("Failed to initialize GameManager");
    }
    StartCoroutine(TurnTakerRoutine());
  }

  public bool RegisterTurnTaker(ITurnTaker turnTaker) {
    return turnTakers.Add(turnTaker);
  }

  private ISet<ITurnTaker> toRemove = new HashSet<ITurnTaker>();
  public bool UnregisterTurnTaker(ITurnTaker turnTaker) {
    return toRemove.Add(turnTaker);
  }

  private IEnumerator TurnTakerRoutine() {
    while (true) {
      yield return new WaitForSeconds(timeBetweenTurns);

      Player.SelectAction(InputManager.S.GetPlayerAction());
      foreach (ITurnTaker turnTaker in turnTakers) {
        if (!toRemove.Contains(turnTaker)) {
          turnTaker.OnTurn();
        }
      }

      turnTakers.ExceptWith(toRemove);
      toRemove.Clear();
    }
  }
}
