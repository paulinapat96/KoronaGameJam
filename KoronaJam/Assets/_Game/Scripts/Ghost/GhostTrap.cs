using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts;
using Gameplay;
using UnityEngine;

public class GhostTrap : MonoBehaviour
{

	[SerializeField] private Collider _myCollider;

	[SerializeField] private GhostAI _GhostPrefab;
	[SerializeField] private List<Transform> _EnemiesToSpawn;

	[SerializeField] private bool _PersistAfterStepOnto;
	
	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			_EnemiesToSpawn?.ForEach(arg =>
			{
				var ghost = Instantiate(_GhostPrefab, arg.position, Quaternion.identity, transform);
				ghost.SetTarget(other.GetComponent<PlayerInteractionController>());
			});

			if (!_PersistAfterStepOnto)
			{
				_myCollider.enabled = false;
			}
		}
	}
}
