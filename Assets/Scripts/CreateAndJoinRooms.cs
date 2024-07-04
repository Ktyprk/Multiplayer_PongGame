using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;


public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField CreateInput;
    public TMP_InputField JoinInput;
    public TMP_InputField NicknameInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(CreateInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
    
    public void SetNickname()
    {
        PhotonNetwork.NickName = NicknameInput.text;
        PlayerPrefs.SetString("PlayerNickname", NicknameInput.text);
    }
}
