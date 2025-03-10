using System;
using System.Runtime.InteropServices;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace _Game.Scripts
{
	public class GhostAI : MonoBehaviour
	{
		
		#pragma disable warning 0414
		
		[Title("Local refs")] 
		[SerializeField] private Animator _Animator;

		[Title("Settings")] 
		[SerializeField] private float _DistanceToTakeDmg = 7;
		[SerializeField] private float _HP = 10;
		[SerializeField] private float _TakenDmgPerSec = 2;
		
		[SerializeField] private float _Speed = .6f;
		[SerializeField] private float _Speed_WhenPlayerLooks = .2f;

		[SerializeField] private float _DistToAttack = 5.0f;
		[SerializeField] private float _AttackFrequency = 3;

		[SerializeField] private float _AngleGreaterThan = 350f;
		[SerializeField] private float _AngleLowerThan = 10f;

		[SerializeField] private PlayerInteractionController _target;

		[SerializeField] private Color _FullHP_Color = new Color(0.77f, 0, 1, .5f);
		[SerializeField] private Color _ZeroHP_Color = new Color(0.77f, 0, 0, 1);

		[SerializeField] private GameObject _AttackParticle;
		[SerializeField] private GameObject _DeathParticle;

		private float _myHP;
		private float _currentSpeed;

		private bool _canInteract = false;
		private float _lastAttackTime;

		private Material _myMaterial;
		
		public void SetTarget(PlayerInteractionController target)
		{
			_target = target;
		}

		public void EnableFollowing()
		{
			
		}

		private void Awake()
		{
			_myHP = _HP;
			_currentSpeed = _Speed;

			var render = GetComponentInChildren<MeshRenderer>();
			_myMaterial = render.material;
			
			// _myMaterial.EnableKeyword("_EMISSION");
			//
			// _myMaterial.SetFloat("_EmissionMap", 1);
			// _myMaterial.SetColor("_EmissionColor", Color.red * 0f);

			_myMaterial.SetColor("_Color", _FullHP_Color);

		}

		public void Evt_GhostActive()
		{
			_canInteract = true;
		}

		public void Update()
		{
			if (_target == null) return;

			if ( PlayerHasFlashlight() && PlayerIsLookingAtMe() && PlayerIsCloseEnough() )
			{
				_myHP -= _TakenDmgPerSec * Time.deltaTime;
				_currentSpeed = _Speed_WhenPlayerLooks;

				var hpProgress = /*1 -*/ (_myHP / _HP);
				_myMaterial.SetColor("_Color", Color.Lerp(_ZeroHP_Color, _FullHP_Color, hpProgress));
				
				if (_myHP < 0)
				{
					KillGhost();
					return;
				}

				return;
			}

			_currentSpeed = _Speed;

			FollowOrAttackPlayer();
			transform.LookAt(_target.gameObject.transform);

		}

		private void KillGhost()
		{
			Instantiate(_DeathParticle, transform.position, Quaternion.identity);
				
			Destroy(gameObject);
		}

		private bool PlayerHasFlashlight()
		{
			return _target.HasFlashlightEnabled;
		}

		private void FollowOrAttackPlayer()
		{
			if (!_canInteract) return;
			
			var distToPlayer = Vector3.Distance(_target.transform.position, transform.position);
			if (distToPlayer > _DistToAttack)
			{
				FollowPlayer();
			}
			else
			{
				AttackPlayer();
			}
		}
		private void FollowPlayer()
		{
			var direction = (_target.transform.position - transform.position).normalized;
			transform.position += direction * (_currentSpeed * Time.deltaTime);
		}

		private void AttackPlayer()
		{
			if (_lastAttackTime + _AttackFrequency < Time.time)
			{
				Debug.Log("Stun!");
				_target.PlayerHasBeenStunned(transform, _AttackParticle);
				
				_lastAttackTime = Time.time;
			}
		}

		private float AngleBetweenMeAndTarget
		{
			get
			{
				var playerForward = _target.transform.forward;
				var ourVector = (transform.position - _target.transform.position).normalized;

				var angle = Vector2.Angle(new Vector2(playerForward.x, playerForward.z), new Vector2(ourVector.x, ourVector.z));
				
				return angle;
			}
		}

		private bool PlayerIsCloseEnough()
		{
			return Vector3.Distance(transform.position, _target.transform.position) < _DistanceToTakeDmg;
		}

		private bool PlayerIsLookingAtMe()
		{
			var angle = AngleBetweenMeAndTarget;
			return angle > _AngleGreaterThan || angle < _AngleLowerThan;
		}
	}
}
