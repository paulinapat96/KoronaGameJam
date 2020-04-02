using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    GameObject holdingItem = null;

    private void OnTriggerStay(Collider other)
    {
        //co jak są dwa obiekty w zasiegu?
        if(Input.GetKeyDown(KeyCode.E))
        { 
            if(other.tag == "Pickup")
            {
                holdingItem = other.gameObject;
                holdingItem.transform.SetParent(transform);
                holdingItem.transform.localPosition = new Vector3(0, 0, 1f);
                Debug.Log("znajdźka");
            }
        }
    }

    private void PickOrDropItem()
    {
        if(holdingItem)
        {

        }
        else
        {

        }
    }
}
