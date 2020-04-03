using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Pickup holdigPickup = null;
    GameObject holdingCraftinItem = null;
    [SerializeField] GameObject pressETextObject;
    private bool isButtonPressedDown = false;
    private float holdTimer = 0f;
    private GameObject currenObjectInCollision = null; // TODO: zamiana na listę obiektów i obsłużenie wiele kolizji jednoczesnie

    
    private void Start()
    {
        ShowText(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup" || other.tag == "CraftinItem")
        {
            currenObjectInCollision = other.gameObject;
            ShowText(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup" || other.tag == "CraftinItem")
        {
            pressETextObject.SetActive(false);
            currenObjectInCollision = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && currenObjectInCollision)
        {
            if (!holdigPickup)
            {
                if (currenObjectInCollision.tag == "Pickup")
                {
                    holdigPickup = currenObjectInCollision.GetComponent<Pickup>();
                    PickItem();
                }

                if (currenObjectInCollision.tag == "CraftingItem" && isButtonPressedDown)
                {

                    //StopHoldingCraftingitem();

                }
            }
            else
            {
                DropItem();
                if(currenObjectInCollision.tag == "CraftingItem")
                {
                    //PutingItemToCraftinitem()
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currenObjectInCollision.tag == "CraftingItem" && !holdigPickup)
            {
                //if(currenObjectInCollision.GetComponent<CraftingItem>().AreRequirementsFullfilled())
                //{
                //    //StartHoldingCraftingitem();
               // isButtonPressedDown = true;
                //}

            }

        }
    }


    private void PickItem()
    {
        // DOTO: Play animator - podniesienie rąk
        holdigPickup.transform.SetParent(transform);
        holdigPickup.transform.localPosition = new Vector3(0, 0, 1f);
        ShowText(false);

    }

    private void DropItem()
    {
        // TODO: Play animator - opuszczenie rąk
        OnTriggerEnter(holdigPickup.Collider);
        holdigPickup.transform.SetParent(null);
        holdigPickup = null;
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

    public void ShowText(bool displayText)
    {
        pressETextObject.SetActive(displayText);
    }
}
