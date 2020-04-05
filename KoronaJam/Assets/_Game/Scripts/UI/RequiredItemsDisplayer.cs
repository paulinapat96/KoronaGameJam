using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

public class RequiredItemsDisplayer : MonoBehaviour
{
    [Title("Global refs")]
    [SerializeField] private RequiredItem _RequiredItemsPrefab;

    [Title("Local refs")] 
    [SerializeField] private Transform _ItemsContainer;

    [Title("Settings")] 
    [SerializeField] private float _DistFromBeginShowing = 5;
    [SerializeField] private float _DistFromFullAlpha = 3;
    
    private Dictionary<Pickup, RequiredItem> _items;
    private CanvasGroup _CanvasGroup;

    private Transform _Player;

    private void Awake()
    {
        _Player = FindObjectOfType<PlayerInteractionController>().transform;
        _CanvasGroup = transform.parent.gameObject.AddComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_CanvasGroup == null) return;

        var distance = Vector3.Distance(transform.position, _Player.position);

        if (distance > _DistFromBeginShowing)
        {
            _CanvasGroup.alpha = 0;
        }
        else if (distance <= _DistFromBeginShowing && distance > _DistFromFullAlpha)
        {
            var alpha = (distance - _DistFromBeginShowing) / (_DistFromFullAlpha - _DistFromBeginShowing);
            _CanvasGroup.alpha = alpha;
        }
        else
        {
            _CanvasGroup.alpha = 1;
        }
        
    }

    public void SetData(List<Pickup> pickups)
    {
        _items = new Dictionary<Pickup, RequiredItem>();
        
        pickups.ForEach(arg =>
        {
            var ob = Instantiate(_RequiredItemsPrefab, Vector3.zero, Quaternion.identity, _ItemsContainer);
            ob.transform.localPosition = Vector3.zero;
            ob.SetSprite(arg.IconDisabled, false);

            _items.Add(arg, ob);
        });
    }
    
    

    public void ReleaseAllObjects()
    {
        for (var i = 0; i < _ItemsContainer.childCount; i++)
        {
            Destroy(_ItemsContainer.GetChild(i));
        }
    }

    public void Show()
    {
        _ItemsContainer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _ItemsContainer.gameObject.SetActive(false);
    }

    public void UpdateState(Pickup receivedItem)
    {
        if (_items.ContainsKey(receivedItem))
        {
            _items[receivedItem].SetSprite(receivedItem.IconEnabled, true);
        }
        else
        {
            Debug.LogError("Brought item which was not expected. WTF");
        }
    }
}
