﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public interface IPlayer : IBallUser, IXzController {

  //An icon representing the player
  Sprite Icon { get; }

  ScoreComponent.PlayerType Team { get; }

  void AButtonDown(XboxController controller);
  void BButtonDown(XboxController controller);
  void XButtonDown(XboxController controller);
  void RTButtonDown(XboxController controller);
  void RTButtonUp(XboxController controller);
}

//Exists only so GetComponent can be used.  When defining variables/fields/properties, please use IPlayer instead.
public abstract class Player : MonoBehaviour, IPlayer {
#pragma warning disable 0649
  [SerializeField] protected BoxCollider grabHitbox;
  [SerializeField] private ScoreComponent.PlayerType _team;
  [SerializeField] private SimpleHealthBar staminaBar;
  [SerializeField] private Sprite _icon;
#pragma warning restore 0649

  [SerializeField] protected AudioClip successfulStun;
  public Transform OriginalParent { get; private set; }

  private PlayerMover xzController;
  private BallUserComponent ballUserComponent;
  public virtual bool CanReceivePass => true;

  public Sprite Icon => _icon;

  public ScoreComponent.PlayerType Team { get; private set; }

  //Specific to Player
  public virtual bool CanRotate => !IsStunned;
  public virtual bool CanMove => !IsStunned;
  public abstract void AButtonDown(XboxController controller);
  public abstract void BButtonDown(XboxController controller);
  public abstract void XButtonDown(XboxController controller);
  public abstract void RTButtonDown(XboxController controller);
  public abstract void RTButtonUp(XboxController controller);

  protected virtual void Awake() {
    OriginalParent = transform.parent;
    Team = _team;
    xzController = GetComponent<PlayerMover>();
    ballUserComponent = GetComponent<BallUserComponent>();
    StartCoroutine(DashRoutine());

    if (grabHitbox == null) {
      Debug.LogError("SmallPlayer.grabHitbox is null.  HMMMM  <_<");
    }
  }

  //Stun functionality
  private bool IsStunned { get; set; }

  protected virtual IEnumerator StunRoutine() {
    yield return new WaitForSeconds(2f);
    IsStunned = false;
  }
  public virtual void Stun() {
    if (!IsStunned) {
      IsStunned = true;
      StartCoroutine(StunRoutine());
    }
  }


  //
  //IXzController
  //

  public virtual float Speed => xzController.Speed * (IsDashing ? 2f : 1f);

  public float X => xzController.X;
  public float Z => xzController.Z;

  public float XLook => xzController.XLook;
  public float ZLook => xzController.ZLook;

  public virtual void Move(float xMove, float zMove) {
    if (CanMove) {
      float oldSpeed = xzController.Speed;
      xzController.Speed = Speed;
      xzController.Move(xMove, zMove);
      xzController.Speed = oldSpeed;
    }
  }
  public void SetRotation(float xLook, float zLook) {
    if (CanRotate) {
      xzController.SetRotation(xLook, zLook);
    }
  }


  //
  //IBallUser
  //

  public bool HasBall => ballUserComponent.HasBall;

  // public void Pass() {
  //   ballUserComponent.Pass();
  // }

  public bool Steal() {
    if (ballUserComponent.Steal(grabHitbox)) {
      GameManager.S.NotifyOfBallOwnership(Team);
      return true;
    } else {
      return false;
    }
  }

  public void HoldBall(IBall ball) {
    GameManager.S.NotifyOfBallOwnership(Team);
    ballUserComponent.HoldBall(ball);
  }

  //
  // Dash Functionality
  //

  private float dashTimer = .9f;
  public bool dashRefillPenalty { get; private set; } = true;
  public bool IsDashing { get; private set; }

  public void StartDashing() {
    if (!dashRefillPenalty) {
      IsDashing = true;
    }
  }

  public void StopDashing() {
    IsDashing = false;
  }

  private IEnumerator DashRoutine() {
    while (true) {
      yield return new WaitForSeconds(0.1f);

      if (IsDashing) {
        PerformDashAction();
        dashTimer -= 0.1f;
        if (dashTimer <= 0f) {
          dashTimer = 0f;
          dashRefillPenalty = true;
          StopDashing();
        }
      } else {
        dashTimer += 0.1f;
        if (dashTimer >= .9f) {
          dashTimer = .9f;
          dashRefillPenalty = false;
        }
      }
      staminaBar.UpdateBar(dashTimer, .9f);
    }
  }

  protected abstract void PerformDashAction(); //will be defined as Steal in SmallPlayer and Stun in TallPlayer
}
