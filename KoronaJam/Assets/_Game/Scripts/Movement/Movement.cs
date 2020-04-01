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

		private bool _movementEnabled = true;
		
		private void Update()
		{
			if (!_movementEnabled) return;

			ProcessMovement();
			ProcessRotation();
		}

		private void ProcessMovement()
		{
			var horizontal = Input.GetAxis("Horizontal");
			var vertical   = Input.GetAxis("Vertical");

			_CharacterController.Move(new Vector3(horizontal, 0, vertical));
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
