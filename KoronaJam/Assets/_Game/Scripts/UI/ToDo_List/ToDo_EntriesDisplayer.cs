using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
		 
		private Dictionary<string, ToDo_Entry> _EntriesDisplayed;

		public void Start()
		{
			_EntriesDisplayed = new Dictionary<string, ToDo_Entry>();
			
			_CraftingItems = FindObjectsOfType<CraftingItem>();
			_CraftingItems.ForEach(arg => arg.OnComplete += Refresh);

			RefreshDisplay();
		}

		private void Refresh()
		{
			RefreshDisplay();
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
