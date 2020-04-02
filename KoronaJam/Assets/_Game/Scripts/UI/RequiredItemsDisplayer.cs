using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RequiredItemsDisplayer : MonoBehaviour
{
    [Title("Global refs")]
    [SerializeField] private RequiredItem _RequiredItemsPrefab;

    [Title("Local refs")] 
    [SerializeField] private Transform _ItemsContainer;

    public void SetData(List<Pickup> pickups)
    {
        pickups.ForEach(arg =>
        {
            var ob = Instantiate(_RequiredItemsPrefab, Vector3.zero, Quaternion.identity, _ItemsContainer);
            ob.SetSprite(arg.IconDisabled, false);
        });
    }

    public void ReleaseAllObjects()
    {
        for (var i = 0; i < _ItemsContainer.childCount; i++)
        {
            Destroy(_ItemsContainer.GetChild(i));
        }
    }
    
}
