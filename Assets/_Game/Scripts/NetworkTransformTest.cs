using Unity.Netcode;
using UnityEngine;

public class NetworkTransform : NetworkBehaviour
{
	void Update()
	{
		if (IsOwner)
		{
			transform.RotateAround(point: GetComponentInParent<Transform>().position, axis: Vector3.up, angle: 100 * Time.deltaTime);
		}
	}
}
