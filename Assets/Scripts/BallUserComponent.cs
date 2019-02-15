﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMover))]
public class BallUserComponent : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private float localHeightToHoldBallAt;
  [SerializeField] private float maxPassDistance;
  [SerializeField] private AudioClip successfulSteal;
  [SerializeField] private AudioClip successfulPass;
#pragma warning restore 0649

  private IXzController xzController; //Needed only for XLook and ZLook
  private IBall heldBall;

  private void Awake() {
    xzController = GetComponent<PlayerMover>();

    if (maxPassDistance == 0) {
      Debug.LogWarning("BallUserComponent.maxPassDistance equals 0.  HMMMM  <_<");
    }
  }

  public bool HasBall { get { return heldBall != null; } }

  public void HoldBall(IBall ball) {
    heldBall = ball;
    if (ball != null) {
      ball.SetParent(transform);
      Vector3 ballPosition = Vector3.zero;
      ballPosition.y = localHeightToHoldBallAt;
      ballPosition.z = transform.lossyScale.z / 2 + ball.Radius;
      ball.SetPosition(ballPosition);
    }
  }

  public bool Steal(BoxCollider grabHitbox) {
    if (HasBall) {
      return false;
    }

    Vector3 selfToHitbox = grabHitbox.center - transform.position;
    RaycastHit[] hits = Physics.BoxCastAll(transform.position, grabHitbox.bounds.extents, selfToHitbox, Quaternion.identity, selfToHitbox.magnitude);
    foreach (RaycastHit h in hits) {
      BallUserComponent other = h.collider.GetComponent<BallUserComponent>();

      if (other != null && other.heldBall != null) {
        HoldBall(other.heldBall);
        other.heldBall = null;
        SoundManager.Instance.Play(successfulSteal);
        return true;
      }
    }

    return false;
  }

  public void Pass() {
    float yGround = 0 + 0.15f;

    Vector3 passOrigin = transform.position;
    passOrigin.y = yGround;
    Vector3 passDirection = new Vector3(xzController.XLook, 0, xzController.ZLook);

    RaycastHit[] hits = Physics.RaycastAll(passOrigin, passDirection, maxPassDistance);
    float nearestPlayerHitDistance = Mathf.Infinity;
    Player nearestPlayerHit = null;
    foreach (RaycastHit hit in hits) {
      Player player = hit.collider.GetComponent<Player>();
      if (player != null && hit.distance < nearestPlayerHitDistance) {
        nearestPlayerHitDistance = hit.distance;
        nearestPlayerHit = player;
      }
    }

    if (nearestPlayerHit != null) {
      nearestPlayerHit.HoldBall(heldBall);
      Debug.Log("Pass Recipient:" + nearestPlayerHit.name);
      heldBall = null;
      SoundManager.Instance.Play(successfulPass);
    } else
    {
      Debug.Log("No Pass Recipient.");
    }
  }
}
