using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			Move();
		}
	}
	void Update()
	{
		transform.position = Position.Value;
	}
	public void Move()
	{
		if (NetworkManager.Singleton.IsServer)
		{
			Vector3 randomPosition = GetRandomPositionOnPlane();
			transform.position = randomPosition;
			Position.Value = randomPosition;
		}
		else
		{
			SubmitPositionRequestServerRpc();
		}
	}

	[ServerRpc]
	void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
	{
		Position.Value = GetRandomPositionOnPlane();
	}


	static Vector3 GetRandomPositionOnPlane()
	{
		return new Vector3(x: Random.Range(-3f, 3f), y: 1f, z: Random.Range(-3f, 3f));
	}
}
