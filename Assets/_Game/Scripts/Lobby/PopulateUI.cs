using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PopulateUI : MonoBehaviour
{
	public TextMeshProUGUI lobbyName;
	public TextMeshProUGUI lobbyCode;
	public TextMeshProUGUI gameMode;
	public TMP_InputField newName;
	public TMP_InputField newPlayerLevel;

	public GameObject playerInfoContainer;
	public GameObject playerInfoPrefab;

	private CurrentLobby _currentLobby;

	private string lobbyId;
	void Start()
	{
		_currentLobby = GameObject.Find("LobbyManager").GetComponent<CurrentLobby>();
		PopulateUIElements();
		lobbyId = _currentLobby.currentLobby.Id;
		InvokeRepeating(nameof(PollForLobbyUpdate), 1.1f, repeatRate: 2f);
		LobbyStatic.LogPlayersInLobby(_currentLobby.currentLobby);
	}

	void PopulateUIElements()
	{
		lobbyName.text = _currentLobby.currentLobby.Name;
		lobbyCode.text = _currentLobby.currentLobby.LobbyCode;
		gameMode.text = _currentLobby.currentLobby.Data["GameMode"].Value;
		ClearContainer();
		foreach (Player player in _currentLobby.currentLobby.Players)
		{
			CreatePlayerInfoCard(player);
		}
	}

	void CreatePlayerInfoCard(Player player)
	{
		GameObject text = Instantiate(playerInfoPrefab, Vector3.zero, Quaternion.identity);
		text.name = player.Joined.ToShortTimeString();
		text.GetComponent<TextMeshProUGUI>().text = player.Id + ":" + player.Data["PlayerLevel"].Value;
		RectTransform rectTransform = text.GetComponent<RectTransform>();
		rectTransform.SetParent(playerInfoContainer.transform);
	}
	private void ClearContainer()
	{
		if (playerInfoContainer is not null && playerInfoContainer.transform.childCount > 0)
		{
			foreach (Transform VARIABLE in playerInfoContainer.transform)
			{
				Destroy(VARIABLE.gameObject);
			}
		}
	}

	async void PollForLobbyUpdate()
	{
		_currentLobby.currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
		PopulateUIElements();
	}

	// Button Events

	public async void ChangeLobbyName()
	{
		string newLobbyName = newName.text;
		try
		{
			UpdateLobbyOptions options = new UpdateLobbyOptions();
			options.Name = newLobbyName;
			_currentLobby.currentLobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyId, options);
		}

		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

	public async void ChangePlayerName()
	{
		string playerlevel = newPlayerLevel.text;
		try
		{
			UpdatePlayerOptions options = new UpdatePlayerOptions();
			options.Data = new Dictionary<string, PlayerDataObject>()
		{
			{"PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerlevel) }
		};
			await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId: AuthenticationService.Instance.PlayerId, options);
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}
}


