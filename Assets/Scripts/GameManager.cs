﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
  public static GameManager S { get; private set; }

#pragma warning disable 0649
  [Range(0.01f, 2f)] [SerializeField] private float timeBetweenTurns = 0.01f;
  [SerializeField] private PlayerObject _playerObject;
  [SerializeField] private BoardObject boardMaker;
  [SerializeField] private int CompletionX = 0;
  [SerializeField] private int CompletionY = 0;
  [SerializeField] private GameObject LevelCompleteUI;
#pragma warning restore 0649

  public Player Player { get; private set; }
  public Board Board { get; private set; }
  public Transform TileInhabitantObjectHolder { get; private set; }

  //This is public so that other objects can calculate how long their animations should take.
  public float TimeBetweenTurns => timeBetweenTurns;

  //Initialized here so that other classes can safely call RegisterTurnTaker inside of Awake
  private ISet<ITurnTaker> turnTakers = new HashSet<ITurnTaker>();

  //Game States so the GameManager knows when to stop and start the TurnTaker Routine.
  private enum GameState
  {
    Running = 0,
    Stopped = 1
  }

  private GameState currentState = GameState.Stopped;


  private void Awake() {
    S = this;
    TileInhabitantObjectHolder = new GameObject().transform;
    TileInhabitantObjectHolder.name = "TileInhabitantObjectHolder";
    boardMaker.Initialize();
    Board = new Board(boardMaker.NumRows, boardMaker.NumCols, boardMaker.tilePrefab, boardMaker.transform);
    boardMaker.PopulateBoard();

    Player = new Player(_playerObject);
    if (Board == null || Player == null) {
      throw new System.Exception("Failed to initialize GameManager");
    }
    currentState = GameState.Running;
    StartCoroutine(TurnTakerRoutine());
  }

  private ISet<ITurnTaker> toAdd = new HashSet<ITurnTaker>();
  public bool RegisterTurnTaker(ITurnTaker turnTaker) {
    return toAdd.Add(turnTaker);
  }

  private ISet<ITurnTaker> toRemove = new HashSet<ITurnTaker>();
  public bool UnregisterTurnTaker(ITurnTaker turnTaker) {
    return toRemove.Add(turnTaker);
  }

  private IEnumerator TurnTakerRoutine() {
    while (currentState == GameState.Running) {
      yield return new WaitForSeconds(timeBetweenTurns);

      Player.SelectAction(InputManager.S.GetPlayerAction());
      turnTakers.UnionWith(toAdd);
      toAdd.Clear();
      foreach (ITurnTaker turnTaker in turnTakers) {
        if (!toRemove.Contains(turnTaker)) {
          turnTaker.OnTurn();
        }
      }

      turnTakers.ExceptWith(toRemove);
      toRemove.Clear();
      checkCompletion();
    }
  }

  public void stopLevel()
  {
    currentState = GameState.Stopped;
    StopCoroutine(TurnTakerRoutine());

    turnTakers.ExceptWith(toRemove);
    toAdd.Clear();
    toRemove.Clear();
    turnTakers.Clear();
  }

  public void LoadMenu()
  {
    SceneManager.LoadScene("MainMenu");
  }
  //TODO: Change this behaviour to load different levels.
  public void LoadNextLevel()
  {
    SceneManager.LoadScene("Level2");
  }

  public void Quit()
  {
    Application.Quit();
  }

  private void revealLevelComplete()
  {
    LevelCompleteUI.SetActive(true);
  }

  private void checkCompletion()
  {
    int col = CompletionX;
    int row = CompletionY;
    Tile t = Board[row, col];
    if (Player.Occupies().Contains(t)) {
      stopLevel();
      revealLevelComplete();
    }
  }

}
