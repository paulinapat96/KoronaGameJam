using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Title("Item data")]
    [SerializeField] private string _ItemName;
    [SerializeField] private Sprite _IconDisabled;
    [SerializeField] private Sprite _IconEnabled;

    [Title("Local refs")]
    [SerializeField] private Collider _Collider;
    public Sprite IconEnabled => _IconEnabled;
    public Sprite IconDisabled => _IconDisabled;
    public string ItemName => _ItemName;
    public Collider Collider => _Collider;

    private void OnDestroy()
    {
        //make some magic
    }
}
