using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressSetter : MonoBehaviour
{

	[SerializeField] private Slider _Slider;

	public void ShowSlider()
	{
		_Slider.gameObject.SetActive(true);
	}

	public void HideSlider()
	{
		_Slider.gameObject.SetActive(false);
	}

	public void SetValue(float val)
	{
		_Slider.value = val;
	}

}
