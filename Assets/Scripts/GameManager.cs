﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
  public static GameManager S { get; private set; }

  public int Score { get; private set; }

#pragma warning disable 0649
  [Range(0.01f, 2f)] [SerializeField] private float timeBetweenTurns = 0.1f;
  [SerializeField] private PlayerObject _playerObject;
  [SerializeField] private BoardObject boardPrefab;
  [SerializeField] private TextAsset level;

  [Header("Level Completion")]
  [SerializeField] private string nextScene = "MainMenu";
  [SerializeField] private int completionRow = 0;
  [SerializeField] private int completionCol = 0;
#pragma warning restore 0649

  public Player Player { get; private set; }
  public Board Board { get; private set; }
  public Transform TileInhabitantObjectHolder { get; private set; }

  //This is public so that other objects can calculate how long their animations should take.
  public float TimeBetweenTurns => timeBetweenTurns;

  //Initialized here so that other classes can safely call RegisterTurnTaker inside of Awake
  private ISet<ITurnTaker> turnTakers = new HashSet<ITurnTaker>();

  //Game States so the GameManager knows when to stop and start the TurnTaker Routine.
  public enum GameState {
    Running,
    Stopped,
    GameOver,
  }
  public GameState CurrentState { get; set; } = GameState.Stopped;

  private void Awake() {
    S = this;
    Score = 10000;
    TileInhabitantObjectHolder = new GameObject().transform;
    TileInhabitantObjectHolder.name = "TileInhabitantObjectHolder";
    boardPrefab = Instantiate(boardPrefab);
    boardPrefab.Initialize(level);
    Board = new Board(boardPrefab.NumRows, boardPrefab.NumCols, boardPrefab.tilePrefab, boardPrefab.transform);
    //boardPrefab.PopulateBoard();

    Player = new Player(_playerObject);
    if (Board == null || Player == null) {
      throw new System.Exception("Failed to initialize GameManager");
    }
    CurrentState = GameState.Running;
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
    while (true) {
      if (CurrentState != GameState.Running) {
        yield return new WaitUntil(() => CurrentState == GameState.Running);
      }

      Score -= 1;
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
      CheckCompletion();
    }
  }


  //
  //Level completion
  //

  public void LoadMenu() {
    SceneManager.LoadScene("MainMenu");
  }

  //TODO: Change this behaviour to load different levels.
  public void LoadNextLevel() {
    SceneManager.LoadScene(nextScene);
  }

  public void Quit() {
    Application.Quit();
  }

  private void CheckCompletion() {
    if (Player.Row >= completionRow && Player.Col >= completionCol) {
      CurrentState = GameState.GameOver;
      StopCoroutine(TurnTakerRoutine());

      turnTakers.ExceptWith(toRemove);
      toAdd.Clear();
      toRemove.Clear();
      turnTakers.Clear();

      BinarySaver.S.SaveCompletion(Score);
    }
  }
}
