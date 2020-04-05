using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
	public class PlayerInteractionController : MonoBehaviour
	{
		[Title("Local refs")] 
		[SerializeField] private PlayerInteraction _PlayerInteraction;
		[SerializeField] private Movement _Movement;

		[Title("Settings")] 
		[SerializeField] private float _StunDuration;

		private float _timeOfUnlock;
		private bool _playerIsStunned = false;
		
		public void PlayerHasBeenStunned()
		{
			// PlayerInteraction	
			_Movement.PlayerHasBeenStunned();

			_timeOfUnlock = Time.time + _StunDuration;
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
