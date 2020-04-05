using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
	public class Movement : MonoBehaviour
	{
		
		#pragma warning disable 0649

		[Title("Global refs")] 
		[SerializeField] private Camera _Camera;
		
		[Title("Local refs")] 
		[SerializeField] private CharacterController _CharacterController;
		[SerializeField] private Animator _PlayerAnimator;

		[Title("Settings")] 
		[SerializeField] private float _SpeedModifier = 1;
		[SerializeField] private float _StunnedSpeedModifier = 1;
		[SerializeField] private float _NonHoldingItem_Center = 0;
		[SerializeField] private float _NonHoldingItem_Radius = 0;
		[SerializeField] private float _HoldingItem_Center = 0;
		[SerializeField] private float _HoldingItem_Radius = 0;

		private float _currentSpeedModifier;
		private bool _movementEnabled = true;
		private bool _playerIsStunned = false;

		public void DisableMovement()
		{
			_movementEnabled = false;
		}

		public void EnableMovement()
		{
			_movementEnabled = true;
		}

		public void PlayerHasBeenStunned()
		{
			_currentSpeedModifier = _StunnedSpeedModifier;
			_playerIsStunned = true;
		}

		public void PlayerRestoredFromStun()
		{
			_currentSpeedModifier = _SpeedModifier;
			_playerIsStunned = false;
		}

		public void PlayerHoldItem()
		{
			_CharacterController.center = new Vector3(0, 0, _HoldingItem_Center);
			_CharacterController.radius = _HoldingItem_Radius;

			_PlayerAnimator.SetBool("HoldItem", true);
		}

		public void PlayerReleasedItem()
		{
			_CharacterController.center = new Vector3(0, 0, _NonHoldingItem_Center);
			_CharacterController.radius = _NonHoldingItem_Radius;
			
			_PlayerAnimator.SetBool("HoldItem", false);
		}

		private void Awake()
		{
			PlayerReleasedItem();
			SetDefaultSpeed();
		}

		private void Update()
		{
			ProcessMovement();
			ProcessRotation();
			ProcessFlashlight();
		}

		private void SetDefaultSpeed()
		{
			_currentSpeedModifier = _SpeedModifier;
		}
		
		private void ProcessMovement()
		{
			if (!_movementEnabled) return;

			var horizontal = Input.GetAxis("Horizontal");
			var vertical   = Input.GetAxis("Vertical");

			var movementOffset = new Vector3(horizontal, 0, vertical);
			
			var normalizedMovementOffset = movementOffset.magnitude > 1 ? movementOffset.normalized : movementOffset;
			
			normalizedMovementOffset -= Vector3.up * 9.81f;
			normalizedMovementOffset *= _currentSpeedModifier * Time.deltaTime;
			
			_CharacterController.Move(normalizedMovementOffset);

			var fwdDotProduct = Vector3.Dot(transform.forward, _CharacterController.velocity);
			var rightDotProduct = Vector3.Dot(transform.right, _CharacterController.velocity);
			
			_PlayerAnimator.SetFloat("Speed", Mathf.Abs(fwdDotProduct));
			_PlayerAnimator.SetFloat("Speed_Horizontal", rightDotProduct);

		}

		private void ProcessRotation()
		{
			if (_playerIsStunned) return;
			
			var mousePosition = Input.mousePosition;
			var trackedPosition = _Camera.WorldToScreenPoint(transform.position);
			
			mousePosition.x -= trackedPosition.x;
			mousePosition.y -= trackedPosition.y;
			
			var angle = 90 - Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
			
			transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
		}

		private void ProcessFlashlight()
		{
			if (_playerIsStunned) return;
			
			
		}

	}
}
