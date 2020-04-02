using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private string _ItemName;
    [SerializeField] private Sprite _IconDisabled;
    [SerializeField] private Sprite _IconEnabled;

    public Sprite IconEnabled => _IconEnabled;
    public Sprite IconDisabled => _IconDisabled;
    public string ItemName => _ItemName;

    private void OnDestroy()
    {
        //make some magic
    }
}
