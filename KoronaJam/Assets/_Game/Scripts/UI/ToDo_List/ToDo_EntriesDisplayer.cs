using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.UI
{
	public class ToDo_EntriesDisplayer : MonoBehaviour
	{
		private CraftingItem[] _CraftingItems;

		[Title("Global refs")]
		[SerializeField] private ToDo_Entry _EntryPrefab;

		[Title("Local refs")] 
		[SerializeField] private Transform _ElementsContainer;
		[SerializeField] private CanvasGroup _CanvasGroup;

		[SerializeField] private GameObject _EndResultScreen;
		[SerializeField] private TextMeshProUGUI _EndTime;
		
		private Dictionary<string, ToDo_Entry> _EntriesDisplayed;

		private float _beginTime;

		public void Start()
		{
			_EntriesDisplayed = new Dictionary<string, ToDo_Entry>();
			
			_CraftingItems = FindObjectsOfType<CraftingItem>();
			_CraftingItems.ForEach(arg => arg.OnComplete += Refresh);

			_beginTime = Time.time;

			RefreshDisplay();
		}

		private void Update()
		{
			_CanvasGroup.alpha = (Input.GetKey(KeyCode.Q))? 1 : 0;
		}

		private void Refresh()
		{
			if (_CraftingItems.All(arg => arg.AlreadyCompleted))
			{
				ShowGoodJob();
			}
			else
			{
				RefreshDisplay();
			}
		}

		private void ShowGoodJob()
		{
			_EndResultScreen.gameObject.SetActive(true);
			_EndTime.text = (Time.time - _beginTime).ToString("000.00") + "s";
		}

		private void RefreshDisplay()
		{
			_CraftingItems.ForEach(arg =>
			{
				if (!arg.IsUnlocked) return;

				if (_EntriesDisplayed.ContainsKey(arg.Name))
				{
					_EntriesDisplayed[arg.Name].SetData(arg.Name,
						arg.AlreadyCompleted ? ToDo_Entry.EntryType.Completed : ToDo_Entry.EntryType.ToDo);
				}
				else
				{
					var newEntry = Instantiate(_EntryPrefab, _ElementsContainer, false);
					newEntry.SetData(arg.Name,
						arg.AlreadyCompleted ? ToDo_Entry.EntryType.Completed : ToDo_Entry.EntryType.ToDo);

					_EntriesDisplayed.Add(arg.Name, newEntry);
				}

			});
		}
	}
}
