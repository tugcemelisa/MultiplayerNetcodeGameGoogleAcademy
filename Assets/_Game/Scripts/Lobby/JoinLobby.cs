using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinLobby : MonoBehaviour
{
	public TMP_InputField InputField;
	public async void JoinLobbyWithLobbyCode(string lobbyCode)
	{
		string code = InputField.text;
		try
		{
			JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
			options.Player = new Player(AuthenticationService.Instance.PlayerId);
			options.Player.Data = new Dictionary<string, PlayerDataObject>()
		{
			{"PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, value:"29") } // aslında static sınıftan getirmek gerek
		};

			Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
			Debug.Log("Joined Lobby With Code : " + code);

			DontDestroyOnLoad(target: this);
			GetComponent<CurrentLobby>().currentLobby = lobby;

			LobbyStatic.LogPlayersInLobby(lobby);
			LobbyStatic.LoadLobbyRoom();
		}
		catch (LobbyServiceException e)
		{
			Debug.LogError(e);
		}
	}

	public async void JoinLobbyWithLobbyId(string lobbyId)
	{
		try
		{
			JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
			options.Player = new Player(AuthenticationService.Instance.PlayerId);
			options.Player.Data = new Dictionary<string, PlayerDataObject>()
		{
			{"PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, value:"29") } // aslında static sınıftan getirmek gerek
		};

			Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);

			DontDestroyOnLoad(target: this);
			GetComponent<CurrentLobby>().currentLobby = lobby;

			Debug.Log("Joined Lobby With ID : " + lobbyId);
			Debug.LogWarning("Lobby Code : " + lobby.LobbyCode);

			LobbyStatic.LogPlayersInLobby(lobby);
			LobbyStatic.LoadLobbyRoom();
		}
		catch (LobbyServiceException e)
		{
			Debug.LogError(e);
		}
	}

	public async void QuickJoinMethod()
	{
		try
		{
			Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
			GetComponent<CurrentLobby>().currentLobby = lobby;
			DontDestroyOnLoad(target: this);
			Debug.Log("Joined Lobby With Quick Join : " + lobby.Id);
			Debug.LogWarning("Lobby Code : " + lobby.LobbyCode);
			LobbyStatic.LoadLobbyRoom();
		}
		catch (LobbyServiceException e)
		{
			Debug.LogError(e);
		}
	}
}
