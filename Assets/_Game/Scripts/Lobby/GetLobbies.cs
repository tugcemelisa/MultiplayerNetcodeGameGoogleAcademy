using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class GetLobbies : MonoBehaviour
{
	public GameObject buttonPrefab;
	public GameObject buttonsContainer;

	async void Start()
	{
		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
	}

	public async void GetLobbiesTest()
	{
		ClearContainer();
		try
		{
			QueryLobbiesOptions options = new();
			Debug.LogWarning(message: "QueryLobbiesTest");

			options.Count = 25;

			options.Filters = new List<QueryFilter>()
			{
				new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GT),
				//new QueryFilter(QueryFilter.FieldOptions.S1,"Librarian", QueryFilter.OpOptions.EQ)

			};

			options.Order = new List<QueryOrder>()
			{
				new QueryOrder(true,QueryOrder.FieldOptions.Created)
			};

			QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
			Debug.LogWarning("Get Lobbies Done COUNT:" + lobbies.Results.Count);
			foreach (Lobby bulunanLobby in lobbies.Results)
			{
				LobbyStatic.LogLobby(bulunanLobby);
				CrateLobbyButton(bulunanLobby);
				//	Debug.Log("Lobby ismi=" + bulunanLobby.Name + "\n" +								
				//		"Lobby oluşturuldu vakti = " + bulunanLobby.Created + "\n" +								
				//	"Lobby Code = " + bulunanLobby.LobbyCode + "\n" +								
				//	"Lobby ID : " + bulunanLobby.Id);								
				//GetComponent<JoinLobby>().JoinLobbyWithLobbyId(lobbies.Results[0].Id);  
			}
		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}
	private void CrateLobbyButton(Lobby lobby)
	{
		GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
		button.name = lobby.Name + "_Button";
		button.GetComponentInChildren<TextMeshProUGUI>().text = lobby.Name;
		RectTransform recTransform = button.GetComponent<RectTransform>();
		recTransform.SetParent(buttonsContainer.transform);
		button.GetComponent<Button>().onClick.AddListener(delegate () { Lobby_OnClick(lobby); });
	}
	public void Lobby_OnClick(Lobby lobby)
	{
		Debug.Log("Clicked Lobby:" + lobby.Name);
		GetComponent<JoinLobby>().JoinLobbyWithLobbyId(lobby.Id);
	}
	private void ClearContainer()
	{
		if (buttonsContainer is not null && buttonsContainer.transform.childCount > 0)
		{
			foreach (Transform VARIABLE in buttonsContainer.transform)
			{
				Destroy(VARIABLE.gameObject);
			}
		}
	}
}
