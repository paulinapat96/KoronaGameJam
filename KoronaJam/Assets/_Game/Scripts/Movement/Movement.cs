using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Movement
{
	public class Movement : MonoBehaviour
	{
		
		#pragma warning disable 0649

		[Title("Global refs")] 
		[SerializeField] private Camera _Camera;
		
		[Title("Local refs")] 
		[SerializeField] private CharacterController _CharacterController;

		[Title("Settings")] 
		[SerializeField] private float _SpeedModifier = 1;
		[SerializeField] private float _NonHoldingItem_Center = 0;
		[SerializeField] private float _NonHoldingItem_Radius = 0;
		[SerializeField] private float _HoldingItem_Center = 0;
		[SerializeField] private float _HoldingItem_Radius = 0;
		

		private bool _movementEnabled = true;

		public void DisableMovement()
		{
			_movementEnabled = false;
		}

		public void EnableMovement()
		{
			_movementEnabled = true;
		}

		public void PlayerHoldItem()
		{
			_CharacterController.center = new Vector3(0, 0, _HoldingItem_Center);
			_CharacterController.radius = _HoldingItem_Radius;

		}

		public void PlayerReleasedItem()
		{
			_CharacterController.center = new Vector3(0, 0, _NonHoldingItem_Center);
			_CharacterController.radius = _NonHoldingItem_Radius;
		}

		private void Awake()
		{
			PlayerReleasedItem();
		}

		private void Update()
		{
			ProcessMovement();
			ProcessRotation();
		}

		private void ProcessMovement()
		{
			if (!_movementEnabled) return;

			var horizontal = Input.GetAxis("Horizontal");
			var vertical   = Input.GetAxis("Vertical");

			var movementOffset = new Vector3(horizontal, 0, vertical);
			
			var normalizedMovementOffset = movementOffset.magnitude > 1 ? movementOffset.normalized : movementOffset;
			
			normalizedMovementOffset -= Vector3.up * 9.81f;
			normalizedMovementOffset *= _SpeedModifier * Time.deltaTime;
			
			_CharacterController.Move(normalizedMovementOffset);
		}

		private void ProcessRotation()
		{
			var mousePosition = Input.mousePosition;
			var trackedPosition = _Camera.WorldToScreenPoint(transform.position);
			
			mousePosition.x -= trackedPosition.x;
			mousePosition.y -= trackedPosition.y;
			
			var angle = 90 - Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
			
			transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
		}

	}
}
