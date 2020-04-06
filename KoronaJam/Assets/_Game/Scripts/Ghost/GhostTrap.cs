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

	[SerializeField] private float _Delay = 0;

	private Transform _objWithColision;
	
	public void OnTriggerEnter(Collider other)
	{
		_objWithColision = other.transform;
		
		if (other.CompareTag("Player"))
		{
			if (_Delay > 0)
				Invoke(nameof(SpawnGhosts), _Delay);
			else
			{
				SpawnGhosts();
			}
		}
	}

	private void SpawnGhosts()
	{
		_EnemiesToSpawn?.ForEach(arg =>
		{
			var ghost = Instantiate(_GhostPrefab, arg.position, Quaternion.identity, transform);
			ghost.SetTarget(_objWithColision.GetComponent<PlayerInteractionController>());
		});

		if (!_PersistAfterStepOnto)
		{
			_myCollider.enabled = false;
		}
	}
}
