﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class CraftingItem : MonoBehaviour
{
    [Title("Name")]
    [SerializeField] private string _Name;

    [Title("Local refs")] [Required]
    [SerializeField] private RequiredItemsDisplayer _ItemsDisplayer;
    [SerializeField] private ProgressSetter _ProgressSetter;
    
    [Title("Scene list")]
    [SerializeField] private List<Pickup> _RequiredItemsToTrigger;

    [Title("Required another items")] 
    [SerializeField] private List<CraftingItem> _RequiredPreviousActions;

    [Title("Triggering - required")] [Required] 
    [SerializeField] private List<AbstractJobResult> _AbstractJobResults;


    [Title("Ghosts")] 
    [SerializeField] private GhostSpawner _GhostSpawner;
    [SerializeField] private List<Transform> _PlacesToSpawnGhosts;

    [Title("Settings")] 
    [SerializeField] private float _CraftingTime;

    [SerializeField] private float _DelayToSpawnGhosts = 0;
    [SerializeField] private float _DelayToUnlockNextLevel = 0;


    public event Action OnComplete;

    public string Name => _Name;

    private bool[] _ownedItems;

    private bool _isHoldingButton;
    private float _passedTime = 0;

    private bool _alreadyCompleted = false;
    public bool AlreadyCompleted => _alreadyCompleted;
    public bool IsUnlocked => !_isLocked;

    private bool _isLocked;

    private void Awake()
    {
        InitializeOwnedItems();
        UnlockIfPossible();

        if ( !_RequiredPreviousActions.IsNullOrEmpty() )
        {
            _ItemsDisplayer.Hide();
        }
    }


    private void UnlockIfPossible()
    {
        if (_RequiredPreviousActions.IsNullOrEmpty())
        {
            Unlock();
        }
        else
        {
            var anyPrevActionNotCompleted = _RequiredPreviousActions.Any(arg => arg.AlreadyCompleted == false);
            if (anyPrevActionNotCompleted)
            {
                Lock();

                _RequiredPreviousActions.ForEach(arg => arg.OnComplete += RefreshUnlockStatus);
            }
            else
            {
                Unlock();
            }
        }
    }

    private void RefreshUnlockStatus()
    {
        if (_RequiredPreviousActions.Any(arg => arg.AlreadyCompleted == false)) return;
        
        Unlock();
    }

    private void Unlock()
    {
        _ItemsDisplayer.Show();
        _ItemsDisplayer.SetData(_RequiredItemsToTrigger);

        _isLocked = false;
    }

    private void Lock()
    {
        _isLocked = true;
    }

    private void InitializeOwnedItems()
    {
        _ownedItems = new bool [_RequiredItemsToTrigger.Count];

#if UNITY_EDITOR
        _RequiredItemsToTrigger.RemoveAll(arg => arg == null);
#endif
    }

    public bool AreRequirementsFullfilled()
    {
        return !_alreadyCompleted && !_isLocked && (_ownedItems.IsNullOrEmpty() || _ownedItems.All(arg => arg == true));
    }

    public int GetIndexOfFirstUnownedItem()
    {
        for (var i = 0; i < _ownedItems.Length; i++)
        {
            if (_ownedItems[i] == false) return i;
        }

        return -1;
    }

    public bool CanPutItem(Pickup item)
    {
        if (item == null) return false;
        if (_RequiredItemsToTrigger == null) return false;
        
        return _RequiredItemsToTrigger.Contains(item);

    }
    
    public bool PutItem(Pickup item)
    {
        Debug.Log(item.name + " wants to be put");

        if (_RequiredItemsToTrigger.Contains(item))
        {
            var index = _RequiredItemsToTrigger.IndexOf(item);
            if (index == -1)
            {
                Debug.LogError("Brough item which isnt required. Something wrong? ");
                return false;
            }

            _ItemsDisplayer.UpdateState(item);
            _ownedItems[index] = true;

            if (_ownedItems.All(arg => arg == true) && _CraftingTime == 0)
            {
                BuildingStarted();
            }

            return true;
        }
        
        return false;
    }

    public void BuildingStarted()
    {
        if (AlreadyCompleted) return;
        
        _ItemsDisplayer.Hide();
     
        if (_CraftingTime > 0)
        {
            _ProgressSetter.ShowSlider();

            _passedTime = 0;
            _isHoldingButton = true;
        }
        else
        {
            TriggerAction();
        }
    }

    public void BuildingFinished()
    {
        _ItemsDisplayer.Hide();
        _ProgressSetter.HideSlider();// Show anim

        _isHoldingButton = false;
    }

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        if (!_isHoldingButton) return;
        
        if (AreRequirementsFullfilled())
        {
            _passedTime += Time.deltaTime;
                
            SetActionProgress( _passedTime );

            if (_passedTime >= _CraftingTime)
            {
                TriggerAction();
            }
        }
    }

    public void SetActionProgress(float time)
    {
        if (_CraftingTime <= 0) return;
        
        var progres = time / _CraftingTime;
        
        _ProgressSetter.SetValue(progres);
    }

    [Button]
    public void TriggerAction()
    {
        _AbstractJobResults?.ForEach(arg => arg.ShowChange());

        _alreadyCompleted = true;
        
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        Invoke(nameof(WaitToSpawnGhosts), _DelayToSpawnGhosts);
        Invoke(nameof(WaitToShowComplete), _DelayToUnlockNextLevel);
        
    }


    private void WaitToSpawnGhosts()
    {
        if (_GhostSpawner == null)
        {
            Debug.LogError($"{name} has no GhostSpawner assigned. there will be no ghosts for it");
            return;
        }

        _GhostSpawner.SpawnGhosts(_PlacesToSpawnGhosts);
    }
    private void WaitToShowComplete()
    {
        OnComplete?.Invoke();
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
