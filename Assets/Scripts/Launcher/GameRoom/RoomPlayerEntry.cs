using Character;
using Photon.Pun;
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

    public void ResetEntryOfPlayer()
    {
        if (!string.IsNullOrEmpty(_characterName))
        {
            GameRoomUI.AvailableCharacters.Add(_characterName);
            Logger.Log($"Count of AvailableCharacters is now {GameRoomUI.AvailableCharacters.Count} because we added {_characterName}");
        }
        _playerPickImage.sprite = null;
        SetPlayerName("");
        _characterName = "";
    }

    public void SetPlayerName(string playerName)
    {
        _playerName.text = playerName;
    }

    public void ClickAvatarButton()
    {
        //Only do something if the player clicks on their own avatar.
        if (PhotonNetwork.IsMasterClient && PlayerNumber != PlayerNumber.Player1) return;
        if (!PhotonNetwork.IsMasterClient && PlayerNumber != PlayerNumber.Player2) return;

        string oldCharacterName = _characterName;
        PickNextCharacter();

        if(_characterName != oldCharacterName)
        {
            string playerNumber = PlayerNumber == PlayerNumber.Player1 ? "1" : "2";
            PlayerPicksPlayableCharacterEvent playerPicksPlayableCharacterEvent = new PlayerPicksPlayableCharacterEvent();
            playerPicksPlayableCharacterEvent.SendPlayerPicksPlayableCharacterEvent(playerNumber, _characterName);
        }
    }

    public void PickNextCharacter()
    {
        Logger.Log($"pick next character from {GameRoomUI.AvailableCharacters.Count} available characters.");
        if(GameRoomUI.AvailableCharacters.Count < 1)
        {
            // there are not enough available characters to change the selection    
            return;
        }
        int characterIndex = GameRoomUI.AvailableCharacters.IndexOf(_characterName);

        if(_characterName == "")
        {
            PickCharacter(GameRoomUI.AvailableCharacters[0]);
        }
        else if (characterIndex > GameRoomUI.AvailableCharacters.Count - 1)
        {
            PickCharacter(GameRoomUI.AvailableCharacters[characterIndex + 1]);
        }
        else
        {
            PickCharacter(GameRoomUI.AvailableCharacters[0]);
        }
    }

    public static string GetNextCharacter(string characterName)
    {
        if (GameRoomUI.AvailableCharacters.Count < 1)
        {
            return characterName;
        }

        int characterIndex = GameRoomUI.AvailableCharacters.IndexOf(characterName);

        if (characterName == "")
        {
            return GameRoomUI.AvailableCharacters[0];
        }
        else if (characterIndex > GameRoomUI.AvailableCharacters.Count - 1)
        {
            return GameRoomUI.AvailableCharacters[characterIndex + 1];
        }
        else
        {
            return GameRoomUI.AvailableCharacters[0];
        }
    }

    public void PickCharacter(string characterName)
    {
        string oldCharacterName = _characterName;
        _characterName = characterName;

        // Update the list with the available characters
        if (!string.IsNullOrEmpty(oldCharacterName))
        {
            GameRoomUI.AvailableCharacters.Add(oldCharacterName);
            Logger.Log($"Added old character {oldCharacterName}. Total characters is now {GameRoomUI.AvailableCharacters.Count}");
        }

        GameRoomUI.AvailableCharacters.Remove(_characterName);

        Logger.Log($"pick character {_characterName}. Total characters is now {GameRoomUI.AvailableCharacters.Count}");

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

    public string GetCharacterName()
    {
        return _characterName;
    }
}
