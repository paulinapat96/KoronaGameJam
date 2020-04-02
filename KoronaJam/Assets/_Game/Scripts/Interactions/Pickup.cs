using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite iconDisabled;
    [SerializeField] Sprite iconEnabled;
    private void OnDestroy()
    {
        //make some magic
    }
}
