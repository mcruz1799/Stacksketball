﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

public class PlayerSelectionGUI : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Text countdown;

  [SerializeField] private Image controller1PlayerImage;
  [SerializeField] private Image controller2PlayerImage;
  [SerializeField] private Image controller3PlayerImage;
  [SerializeField] private Image controller4PlayerImage;

  [SerializeField] private RawImage controller1X;
  [SerializeField] private RawImage controller1Ball;
  [SerializeField] private RawImage controller2X;
  [SerializeField] private RawImage controller2Ball;
  [SerializeField] private RawImage controller3X;
  [SerializeField] private RawImage controller3Ball;
  [SerializeField] private RawImage controller4X;
  [SerializeField] private RawImage controller4Ball;

  [SerializeField] private AudioClip selectSound;
  
#pragma warning restore 0649

  private void Awake() {
    countdown.text = "";
  }

  private void Update() {
    UpdateGUI();
  }

  private void UpdateGUI() {
    if (InputManager.S.ControllerMapComplete()) {
      countdown.text = InputManager.S.Countdown.ToString();
    } else {
      countdown.text = "";
    }

    foreach (KeyValuePair<XboxController, InputManager.PlayerSelectionInfo> info in InputManager.S.ControllerMap) {
      Sprite sprite = info.Value.player.Icon;
      bool isConfirmed = info.Value.isConfirmed;

      switch (info.Key) {
        case XboxController.First:
          controller1PlayerImage.sprite = sprite;
          if (isConfirmed) 
          {
             controller1Ball.gameObject.SetActive(true);
             controller1X.gameObject.SetActive(false);
             SoundManager.Instance.Play(selectSound);
          }
          else
          {
             controller1Ball.gameObject.SetActive(false);
             controller1X.gameObject.SetActive(true);
          }
          break;
        case XboxController.Second:
          controller2PlayerImage.sprite = sprite;
          if (isConfirmed) 
          {
             controller2Ball.gameObject.SetActive(true);
             controller2X.gameObject.SetActive(false);
             SoundManager.Instance.Play(selectSound);
          }
          else
          {
             controller2Ball.gameObject.SetActive(false);
             controller2X.gameObject.SetActive(true);
          }
          break;
        case XboxController.Third:
          controller3PlayerImage.sprite = sprite;
          if (isConfirmed) 
          {
             controller3Ball.gameObject.SetActive(true);
             controller3X.gameObject.SetActive(false);
             SoundManager.Instance.Play(selectSound);
          }
          else
          {
             controller3Ball.gameObject.SetActive(false);
             controller3X.gameObject.SetActive(true);
          }
          break;
        case XboxController.Fourth:
          controller4PlayerImage.sprite = sprite;
          if (isConfirmed) 
          {
             controller4Ball.gameObject.SetActive(true);
             controller4X.gameObject.SetActive(false);
             SoundManager.Instance.Play(selectSound);
          }
          else
          {
             controller4Ball.gameObject.SetActive(false);
             controller4X.gameObject.SetActive(true);
          }
          break;
        default:
          Debug.LogError("Illegal XboxController key detected in InputManager.ControllerMap");
          break;
      }
    }
  }
}
