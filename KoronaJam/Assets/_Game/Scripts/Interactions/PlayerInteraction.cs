using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    GameObject holdingItem = null;
    GameObject holdingCraftinItem = null;
    [SerializeField] GameObject pressETextObject;
    private bool isButtonPressedDown = false;
    private float holdTimer = 0f;
    private void Start()
    {
        pressETextObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            pressETextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup") // TODO: dopisać tutaj i do enter potem bluprint
        {
            pressETextObject.SetActive(false);
            Debug.Log("wyjscie - co tu sie odpierdala?");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //co jak są dwa obiekty w zasiegu?
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!holdingItem)
            {
                if (other.tag == "Pickup")
                {
                    holdingItem = other.gameObject;
                    PickItem();
                }
                
                if(isButtonPressedDown)
                {
                    isButtonPressedDown = false;
                    holdTimer = Time.time - holdTimer;
                    HoldingCraftingItem(holdingCraftinItem, holdTimer);
                    holdingCraftinItem = null;
                }
            }
            else 
            {
                DropItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (other.tag == "CraftingItem" && !holdingItem)
            {
                //do somtething on hold E
                isButtonPressedDown = true;
                holdingCraftinItem = other.gameObject;
                holdTimer = Time.time;
            }

        }
    }

    private void PickItem()
    {
        // DOTO: Play animator - podniesienie rąk
        holdingItem.transform.SetParent(transform);
        holdingItem.transform.localPosition = new Vector3(0, 0, 0.5f);
        pressETextObject.SetActive(false);
    }

    private void DropItem()
    {
        // TODO: Play animator - opuszczenie rąk
        //akcje do puszcenia...
        holdingItem.transform.SetParent(null);
        // holdingItem.transform.SetParent(holdingItem.GetComponent<Pickup>().containerTransform.transform); // TODO: przeparentować potem na jakiś kontener
        holdingItem = null;
    }

    private void HoldingCraftingItem(GameObject item, float time)
    {
        Debug.Log(item.name + time);
    }

    public void OnChangeActiveItem()
    {
        // TODO: wyświetlenie aktywnego itemu
     ///   holdingItem.GetComponent<Pickup>().iconDisabled;
    }
}
