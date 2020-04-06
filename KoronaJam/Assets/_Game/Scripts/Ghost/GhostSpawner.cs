using System;
using System.Collections.Generic;
using _Game.Scripts;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace Gameplay
{
	public class GhostSpawner : MonoBehaviour
	{
		[Title("Global refs")] 
		[SerializeField] private PlayerInteractionController _Player;

		[Title("Spawning settings")] 
		[SerializeField] private Vector2 _SpawningTimeRange;
		
		[Title("Ghost prefab")] 
		[SerializeField] private GhostAI _GhostPrefab;
		
		[Title("Spawn points")] 
		[SerializeField] private List<Transform> _PossibleSpawnPoints;

		private bool _spawningEnabled = false;

		private float _timeToNextSpawn;

		public void EnableSpawning()
		{
			_spawningEnabled = true;

			UpdateNextSpawnTime();
		}
		public void DisableSpawning() => _spawningEnabled = false;
		
		public void Start()
		{
			EnableSpawning();	
		}

		private void Update()
		{
			if (!_spawningEnabled) return;

			if (Time.time > _timeToNextSpawn)
			{
				SpawnNewGhost();
				UpdateNextSpawnTime();
			}
			
		}

		private void UpdateNextSpawnTime()
		{
			_timeToNextSpawn = Time.time + Random.Range(_SpawningTimeRange.x, _SpawningTimeRange.y);
		}

		private void SpawnNewGhost()
		{
			var numOfSpawnPoints = _PossibleSpawnPoints.Count;
			var randomPoint = Random.Range(0, numOfSpawnPoints);
			var randomPosition = _PossibleSpawnPoints[randomPoint].position;
			
			SpawnGhostAtPosition(randomPosition);
		}

		private void SpawnGhostAtPosition(Vector3 randomPosition)
		{
			var ghost = Instantiate(_GhostPrefab, randomPosition, Quaternion.identity, null);
			// ghost.transform.localPosition = Vector3.zero;

			ghost.SetTarget(_Player);
		}

		public void SpawnGhosts(List<Transform> placesToSpawnGhosts)
		{
			if (placesToSpawnGhosts.IsNullOrEmpty()) return;
			
			placesToSpawnGhosts.ForEach(arg =>
			{
				if (arg == null) return;
				SpawnGhostAtPosition(arg.position);
			});
		}
	}
}
