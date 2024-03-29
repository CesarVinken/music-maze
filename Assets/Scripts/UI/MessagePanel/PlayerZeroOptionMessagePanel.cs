﻿using Character;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerZeroOptionMessagePanel : PlayerMessagePanel
    {
        public static Dictionary<MessagePosition, Vector2> MessageSpawnPosition = new Dictionary<MessagePosition, Vector2>()
        {
            { MessagePosition.Middle, new Vector2(0, 0) },
            { MessagePosition.PlayerLeft, new Vector2(0, 0) },
            { MessagePosition.PlayerRight, new Vector2(0, 0) }
        };

        [SerializeField] private Text _messageText;

        public void Initialise(string message, PlayerNumber playerNumber)
        {
            if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
            {
                RectTransform rt = gameObject.GetComponent<RectTransform>();

                float fullCanvasWidth = 1280;
                rt.sizeDelta = new Vector2(fullCanvasWidth / 2, rt.sizeDelta.y);

                if (playerNumber == PlayerNumber.Player1)
                {
                    rt.localPosition = new Vector3(-(fullCanvasWidth / 4), rt.localPosition.y, rt.localPosition.z);
                }
                else
                {
                    rt.localPosition = new Vector3(fullCanvasWidth - fullCanvasWidth / 4 * 3, rt.localPosition.y, rt.localPosition.z);
                }
            }
            _messageText.text = message;
            gameObject.SetActive(true);

            MainScreenOverlayCanvas.Instance.OpenMessagePanels.Add(this);
        }

        public void Close()
        {
            MainScreenOverlayCanvas.Instance.OpenMessagePanels.Remove(this);
            Destroy(gameObject);
        }
    }
}