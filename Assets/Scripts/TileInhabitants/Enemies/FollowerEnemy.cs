using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemySubEntity : EnemySubEntity<FollowerEnemy, FollowerEnemySubEntity> {
  private Direction AttackDirection => parent.XVelocity > 0 ? Direction.East : Direction.West;

  public FollowerEnemySubEntity(SingleTileEntityObject gameObject, FollowerEnemy parent, out bool success) : base(gameObject, parent, out success) { }

  public override void Attack() {
    Tile t = GameManager.S.Board.GetInDirection(Row, Col, AttackDirection);
    if (t != null) {
      foreach (ITileInhabitant other in t.Inhabitants) {
        IDamageable victim = other is IDamageable ? (IDamageable)other : null;
        if (victim != null && CanAttack(other)) {
          victim.OnAttacked(parent.AttackPower, AttackDirection);
        }
      }
    }
  }

  private bool CanAttack(ITileInhabitant other) {
    return other is IPlayer && !toIgnore.Contains(other);
  }
}

public class FollowerEnemy : Enemy<FollowerEnemy, FollowerEnemySubEntity> {
  private readonly FollowerEnemyObject gameObject;
  private bool isFollowing;

  private readonly int homeTileRow;
  private readonly int homeTileCol;

  private FollowerEnemy(FollowerEnemyObject gameObject, int homeTileRow, int homeTileCol, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
    this.homeTileRow = homeTileRow;
    this.homeTileCol = homeTileCol;
    XVelocity = 1;
  }

  public override void OnTurn() {
    //If player is close enough, we will follow the player >:)
    if (Mathf.Abs(GameManager.S.Player.Col - homeTileCol) <= gameObject.aggroRange && Mathf.Abs(GameManager.S.Player.Row - homeTileRow) <= gameObject.yAggroRange) {
      isFollowing = true;
    } else {
      isFollowing = false;
    }

    ////If we are too far from home point, return to home
    //if (Mathf.Abs(homeTileCol - TopLeft.Col) > gameObject.maxDistFromHome) {
    //  isFollowing = false;
    //}

    if (isFollowing) {
      FollowPlayer();
    } else {
      ReturnToHome();
    }

    base.OnTurn();
  }

  protected override FollowerEnemySubEntity CreateSubEntity(EnemyObject e, int row, int col, out bool success) {
    SingleTileEntityObject subentityGameObject = new GameObject().AddComponent<SingleTileEntityObject>();
    subentityGameObject.name = string.Format("FollowerEnemy[r={0}, c={1}]", row, col); ;
    subentityGameObject.spawnRow = row;
    subentityGameObject.spawnCol = col;
    subentityGameObject.transform.parent = e.transform;
    return new FollowerEnemySubEntity(subentityGameObject, this, out success);
  }

  protected override void OnCollision(Direction moveDirection) {
    //Do nothing
  }

  private void FollowPlayer() {
    //Check which direction we need to go and change XVelocity accordingly
    int distanceToPlayer = GameManager.S.Player.Col - TopLeft.Col;
    XVelocity = System.Math.Sign(distanceToPlayer);
  }

  private void ReturnToHome() {
    //If we are home, don't move
    //Otherwise, move towards the player in the x-direction only
    if (TopLeft.Row == homeTileRow && TopLeft.Col == homeTileCol) {
      XVelocity = 0;
      YVelocity = 0;
    } else {
      //Check which direction we need to go and change XVelocity accordingly
      int distanceToHome = homeTileCol - TopLeft.Col;
      XVelocity = System.Math.Sign(distanceToHome);
    }
  }

  public static FollowerEnemy Make(FollowerEnemyObject followerEnemyPrefab, int row, int col, Transform parent = null) {
    followerEnemyPrefab = Object.Instantiate(followerEnemyPrefab);
    followerEnemyPrefab.transform.parent = parent;
    followerEnemyPrefab.spawnRow = row;
    followerEnemyPrefab.spawnCol = col;
    FollowerEnemy result = new FollowerEnemy(followerEnemyPrefab, row, col, out bool success);
    return success ? result : null;
  }
}
