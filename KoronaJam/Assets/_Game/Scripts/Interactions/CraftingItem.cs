using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class CraftingItem : MonoBehaviour
{
    [Title("Name")]
    [SerializeField] private string Name;

    [Title("Local refs")] [Required]
    [SerializeField] private RequiredItemsDisplayer _ItemsDisplayer;
    [SerializeField] private ProgressSetter _ProgressSetter;
    
    [Title("Scene list - required")] [Required]
    [SerializeField] private List<Pickup> _RequiredItems;

    [Title("Triggering - required")] [Required] 
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

    public bool AreRequirementsFullfilled()
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

    public void BuildingStarted()
    {
        _ItemsDisplayer.Hide();
        _ProgressSetter.ShowSlider();
    }

    public void BuildingFinished()
    {
        _ItemsDisplayer.Hide();
        _ProgressSetter.HideSlider();// Show anim
        
        TriggerAction();
    }
    
    public void SetActionProgress(float time)
    {
        // TODO: Nie obsłużyłeś on trigger exit ;) Zostaje pasek ładowania 
        if (_CraftingTime <= 0) return;
        
        var progres = time / _CraftingTime;
        
        _ProgressSetter.SetValue(progres);
    }

    [Button]
    public void TriggerAction()
    {
        _AbstractJobResult.ShowChange();
    }

    [Button]
    public void TriggerBuildingAction()
    {
        BuildingStarted();
        
        StartCoroutine(Editor_BuildAction(BuildingFinished));
    }

    private IEnumerator Editor_BuildAction(Action action)
    {
        float animTime = _CraftingTime;
        float currTime = 0;

        while (currTime < animTime)
        {
            SetActionProgress(currTime);

            currTime += Time.deltaTime;

            yield return null;
        }
        
        action?.Invoke();

    }
}
