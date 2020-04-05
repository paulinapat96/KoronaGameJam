using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace Gameplay
{
	public class PlayerInteractionController : MonoBehaviour
	{
		[Title("Local refs")] 
		[SerializeField] private PlayerInteraction _PlayerInteraction;
		[SerializeField] private Movement _Movement;

		[Title("Settings")] 
		[SerializeField] private float _StunDuration;
		[SerializeField] private float _AfterStunProtection = 3;

		private float _timeOfUnlock;
		private bool _playerIsStunned = false;

		private float _timeOfNextPossibleStun;
		
		public void PlayerHasBeenStunned()
		{
			if (Time.time < _timeOfNextPossibleStun) return;
			
			// PlayerInteraction	
			_Movement.PlayerHasBeenStunned();

			_timeOfUnlock = Time.time + _StunDuration;
			_timeOfNextPossibleStun = _timeOfUnlock + _AfterStunProtection;
			_playerIsStunned = true;
		}

		private void Update()
		{
			if (_playerIsStunned)
			{
				if (_timeOfUnlock < Time.time)
				{
					Debug.Log("Restored from stun");
					_Movement.PlayerRestoredFromStun();
					_playerIsStunned = false;
				}
			}
		}
	}
}
