using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemy : Enemy
{
  private readonly FollowerEnemyObject e;
  private bool isFollowing;

  //to be initialized by the level generator
  private int homeTileRow;
  private int homeTileCol;


  protected override Direction AttackDirection => XVelocity > 0 ? Direction.East : Direction.West;

  private FollowerEnemy(FollowerEnemyObject e, int homeRow, int homeCol) : base(e) {
    this.e = e;
    this.homeTileRow = homeRow;
    this.homeTileCol = homeCol;
    XVelocity = 1;
  }

  public override void OnTurn() {
    Move();
    base.OnTurn();
  }

  private void Move() {
    if (isFollowing){
        FollowPlayer();
        return;
    }
    ReturnToHome();
  }

  private void FollowPlayer(){
    if(Mathf.Abs(homeTileCol - this.Col) > e.maxDistFromHome){
      //If we are too far from home point, return to home
      isFollowing = false;
      return;
    }
    //Check which direction we need to go and change XVelocity accordingly
    int distanceToPlayer = GameManager.S.Player.Col - this.Col;
    XVelocity = Mathf.Abs(XVelocity);
    if(distanceToPlayer < 0) XVelocity *= -1;
    else if (distanceToPlayer == 0){
      //XVelocity = 0;
      return;
    }
    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, 0);
    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      if (!CanSetPosition(newRow, newCol)) {
        //change this
        return;
      }

      SetPosition(newRow, newCol, out bool success);
      if (!success) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
  }

  private void ReturnToHome(){
    //If player is close enough, we will follow the player >:)
    if(Mathf.Abs(GameManager.S.Player.Col - this.Col) <= e.aggroRange && Mathf.Abs(GameManager.S.Player.Row - this.Row) <= e.aggroRange){
        isFollowing = true;
        return;
    }

    //If we are home, don't do anything 
    if(this.Row == homeTileRow && this.Col == homeTileCol) return;

    //Check which direction we need to go and change XVelocity accordingly
    int distanceToHome = homeTileCol - this.Col;
    XVelocity = Mathf.Abs(XVelocity);
    if(distanceToHome < 0) XVelocity *= -1;
    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, 0);
    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      if (!CanSetPosition(newRow, newCol)) {
        //We just have to wait until this tile opens up :/
        return;
      }

      SetPosition(newRow, newCol, out bool success);
      if (!success) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
  }

  public static FollowerEnemy Make(FollowerEnemyObject followerEnemyPrefab, int row, int col, Transform parent = null) {
    followerEnemyPrefab = Object.Instantiate(followerEnemyPrefab);
    followerEnemyPrefab.transform.parent = parent;
    followerEnemyPrefab.spawnRow = row;
    followerEnemyPrefab.spawnCol = col;
    return new FollowerEnemy(followerEnemyPrefab, row, col);
  }

  protected override void OnDeath() {
    Object.Destroy(e.gameObject);
    GameManager.S.Board[Row, Col].Remove(this);
    GameManager.S.UnregisterTurnTaker(this);
  }
}
