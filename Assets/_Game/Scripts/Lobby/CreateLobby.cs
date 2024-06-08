using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
	public TMP_InputField lobbyname;
	public TMP_InputField lobbyCode;
	public TMP_Dropdown maxplayers;
	public TMP_Dropdown gamemode;
	public Toggle islobbyprivate;

	public async void CreateLobbyMethod()
	{
		string lobbyName = lobbyname.text;
		int maxPlayers = Convert.ToInt32(maxplayers.options[maxplayers.value].text);
		CreateLobbyOptions options = new CreateLobbyOptions();
		options.IsPrivate = islobbyprivate.isOn;

		//Player Creation
		options.Player = new Player(AuthenticationService.Instance.PlayerId);
		options.Player.Data = new Dictionary<string, PlayerDataObject>()
		{
			{"PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, value:"8") }
		};

		//Lobby Data

		options.Data = new Dictionary<string, DataObject>()
		{
			{
				"GameMode",
				new DataObject(DataObject.VisibilityOptions.Public, gamemode.options[gamemode.value].text, //içerik degistiren game button konulabilir
				DataObject.IndexOptions.S1)
			}
		};

		Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

		GetComponent<CurrentLobby>().currentLobby = lobby;
		DontDestroyOnLoad(target: this);
		Debug.Log("Create Lobby Done!");

		LobbyStatic.LogLobby(lobby);
		LobbyStatic.LogPlayersInLobby(lobby);
		lobbyCode.text = lobby.LobbyCode;

		StartCoroutine(routine: HeartbeatLobbyCoroutine(lobby.Id, waitTimeSeconds: 15f));

		LobbyStatic.LoadLobbyRoom();

	}
	IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
	{
		TimeSpan delay = TimeSpan.FromSeconds(waitTimeSeconds);
		while (true)
		{
			LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
			yield return delay;
		}
	}
}


