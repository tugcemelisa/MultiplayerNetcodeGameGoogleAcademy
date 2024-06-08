using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TestNetworkVariableSynchronization : NetworkBehaviour
{
	private NetworkVariable<int> m_SomeValue = new NetworkVariable<int>();
	private const int k_InitialValue = 1111;

	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			m_SomeValue.Value = k_InitialValue;
			NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		}
		else
		{
			if (m_SomeValue.Value != k_InitialValue)
			{
				Debug.LogWarning($"NetworkVariable was {m_SomeValue.Value} upon being spawned" +
					$" when it should have been {k_InitialValue}");
			}
			else
			{
				Debug.Log($"NetworkVariable is {m_SomeValue.Value} when spawned.");
			}
			m_SomeValue.OnValueChanged += OnSomeValueChanged;
		}
	}

	private void NetworkManager_OnClientConnectedCallback(ulong obj)
	{
		StartCoroutine(StartChangingNetworkVariable());
	}

	private void OnSomeValueChanged(int previous, int current)
	{
		Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
	}

	private IEnumerator StartChangingNetworkVariable()
	{
		int count = 0;
		WaitForSeconds updateFrequency = new WaitForSeconds(0.5f);
		while (count < 4)
		{
			m_SomeValue.Value += m_SomeValue.Value;
			yield return updateFrequency;
		}
		NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
	}
}