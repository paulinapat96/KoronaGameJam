using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor.U2D;
using UnityEngine;

public class CraftingItem : MonoBehaviour
{

    [Title("Local refs")] 
    [SerializeField] private RequiredItemsDisplayer _ItemsDisplayer;
    
    [Title("Scene list")]
    [SerializeField] private List<Pickup> _RequiredItems;

    [Title("Triggering")] 
    [SerializeField] private AbstractJobResult _AbstractJobResult;

    [Title("Settings")] 
    [SerializeField] private float _CraftingTime;

    private bool[] _ownedItems;

    private void Awake()
    {
        _ownedItems = new bool [_RequiredItems.Count];

        #if UNITY_EDITOR
        _RequiredItems.RemoveAll(arg => arg == null);
        #endif
        
        
        if ( !_RequiredItems.IsNullOrEmpty() ) _ItemsDisplayer.SetData(_RequiredItems);
    }

    public bool RequirementsComplete()
    {
        return _ownedItems.All(arg => arg == true);
    }

    public int GetIndexOfFirstUnownedItem()
    {
        for (var i = 0; i < _ownedItems.Length; i++)
        {
            if (_ownedItems[i] == false) return i;
        }

        return -1;
    }
    
    public bool PutItem(Pickup item)
    {
        if (_RequiredItems.Contains(item))
        {
            var index = _RequiredItems.IndexOf(item);
            if (index == -1)
            {
                Debug.LogError("Brough item which isnt required. Something wrong? ");
                return false;
            }

            _ownedItems[index] = true;

            return true;
        }
        
        return false;
        
    }

    [Button]
    public void TriggerAction()
    {
        _AbstractJobResult.ShowChange();
    }
    
}
