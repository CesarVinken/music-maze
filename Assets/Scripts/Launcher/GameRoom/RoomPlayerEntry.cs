using CharacterType;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerEntry : MonoBehaviourPunCallbacks
{
    public PlayerNumber PlayerNumber;

    [SerializeField] private Button _playerPickButton;
    [SerializeField] private Image _playerPickImage;
    [SerializeField] private Text _playerName;

    [SerializeField] private GameRoomUI _gameRoomUI;

    [SerializeField] private Sprite[] _characterAvatars;

    private string _characterName;


    public void SetPlayerName(string playerName)
    {
        _playerName.text = playerName;
    }

    public void PickNextCharacter()
    {
        Logger.Log("pick next character");
        switch (_characterName)
        {
            case "Emmon":
                PickCharacter("Fae");
                break;
            case "Fae":
                PickCharacter("Emmon");
                break;
            default:
                Logger.Warning($"Unknown character type {_characterName}");
                break;
        }
    }

    public void PickCharacter(string characterName)
    {
        _characterName = characterName;

        Logger.Log($"pick character {_characterName}");

        switch (_characterName)
        {
            case "Emmon":
                _playerPickImage.sprite = _characterAvatars[0];
                break;
            case "Fae":
                _playerPickImage.sprite = _characterAvatars[1];
                break;
            default:
                Logger.Warning($"Unknown character type {characterName}");
                break;
        }
    }
}
