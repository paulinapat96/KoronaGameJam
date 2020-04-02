using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationJobResult : AbstractJobResult
{
	[Title("Animator")]
	[SerializeField] private Animator _Animator;

	[Title("Trigger name - optional")] 
	[SerializeField] private string _TriggerName = "proceed";
	
	public override void ShowChange()
	{
		_Animator.SetTrigger(_TriggerName);
	}
}
