using TMPro;
using Unity.Netcode;
using UnityEngine;


public class ButtonActions : MonoBehaviour
{
	private NetworkManager NetworkManager;
	public TextMeshProUGUI text;

	void Start()
	{
		NetworkManager = GetComponentInParent<NetworkManager>();
	}

	public void StartHost()
	{
		NetworkManager.StartHost();
		InitMovementText();
	}

	public void StartClient()
	{
		NetworkManager.StartClient();
		InitMovementText();
	}

	public void SubmitNewPosition()
	{
		NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
		PlayerMovement player = playerObject.GetComponent<PlayerMovement>();
		player.Move();
	}

	private void InitMovementText()
	{
		if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
		{
			text.text = "MOVE";
		}
		else if (NetworkManager.Singleton.IsClient)
		{
			text.text = "Request Move";
		}
	}
}
