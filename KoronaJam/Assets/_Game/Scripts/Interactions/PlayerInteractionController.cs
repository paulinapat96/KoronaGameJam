using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class PlayerInteractionController : MonoBehaviour
	{
		[Title("Local refs")] 
		[SerializeField] private PlayerInteraction _PlayerInteraction;
		[SerializeField] private Movement _Movement;
		[SerializeField] private GameObject _Flashlight;

		[SerializeField] private GameObject _StunSection;
		[SerializeField] private Image _StunCircle;
		
		[Title("Settings")] 
		[SerializeField] private float _StunDuration;
		[SerializeField] private float _AfterStunProtection = 3;

		private float _timeOfStun;
		private float _timeOfUnlock;
		private bool _playerIsStunned = false;

		private float _timeOfNextPossibleStun;
		public bool HasFlashlightEnabled => _isUsingFlashlight;
		private bool _isUsingFlashlight;

		public void PlayerHasBeenStunned(Transform fromWho, GameObject attackParticle)
		{
			if (Time.time < _timeOfNextPossibleStun) return;
			
			// PlayerInteraction	
			_Movement.PlayerHasBeenStunned(fromWho);

			Instantiate(attackParticle, _Movement.transform.position, Quaternion.identity);

			_timeOfStun = Time.time;
			_timeOfUnlock = Time.time + _StunDuration;
			_timeOfNextPossibleStun = _timeOfUnlock + _AfterStunProtection;
			_playerIsStunned = true;
		}

		private void Update()
		{
			_isUsingFlashlight = false;
			
			if (_playerIsStunned)
			{
				HandleStunning();
			}
			else
			{
				if (Input.GetMouseButton(1))
				{
					_isUsingFlashlight = true;
				}
			}
			
			_Flashlight.gameObject.SetActive(_isUsingFlashlight);
			if (_playerIsStunned) _StunCircle.transform.rotation = Quaternion.Euler(0, -transform.rotation.y, 0);
		}

		private void HandleStunning()
		{
			if (_timeOfUnlock < Time.time)
			{
				Debug.Log("Restored from stun");
				_Movement.PlayerRestoredFromStun();
				_playerIsStunned = false;
				_StunSection.SetActive(false);
			}
			else
			{
				var stunProgress = (Time.time - _timeOfStun) / (_timeOfUnlock - _timeOfStun);
				
				_StunSection.SetActive(true);
				_StunCircle.fillAmount = stunProgress;
			}
		}
	}
}
