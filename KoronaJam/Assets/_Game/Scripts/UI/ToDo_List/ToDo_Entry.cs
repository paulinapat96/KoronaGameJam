using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.UI
{
	public class ToDo_Entry : MonoBehaviour
	{
		[Title("Local refs")]
		[SerializeField] private TextMeshProUGUI _Entry;

		public enum EntryType { ToDo, Completed,  }

		private string _displayedString;
		public string DisplayedString => _displayedString;
		public void SetData(string data, EntryType type )
		{
			_displayedString = data;
			
			if (type == EntryType.ToDo)
			{
				_Entry.text = $"{data}";
			}
			
			if (type == EntryType.Completed)
			{
				_Entry.text = $"<s>{data}</s>";
			}
			
		}
		
	}
}
