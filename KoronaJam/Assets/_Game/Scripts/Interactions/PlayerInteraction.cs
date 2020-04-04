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
        if (other.CompareTag("Pickup"))
        {
            currenObjectInCollision = other.gameObject;
            ShowText(true);
        }
        else if (other.CompareTag("CraftingItem"))
        {
            var craftingItem = other.GetComponent<CraftingItem>();
            currenObjectInCollision = other.gameObject;
            
            if (craftingItem.IsUnlocked)
            {
                ShowText(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pickup" || other.tag == "CraftingItem")
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

                    currenObjectInCollision.GetComponent<CraftingItem>().BuildingFinished();
                    Debug.Log("stop budowy");

                }
            }
            else
            {
                if(currenObjectInCollision.tag == "CraftingItem")
                {
                    if(currenObjectInCollision.GetComponent<CraftingItem>().PutItem(holdigPickup))
                    {
                        Debug.Log("udało się wsadzić item");
                        holdigPickup.gameObject.transform.SetParent(currenObjectInCollision.transform);
                        holdigPickup.gameObject.SetActive(false);
                        holdigPickup = null;
                        return;

                    }
                    else
                    {
                        Debug.Log("nie mozesz wsadzić itemu");
                    }
                  
                }

                if(holdigPickup) DropItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currenObjectInCollision && currenObjectInCollision.CompareTag("CraftingItem") && !holdigPickup)
            {  
                CraftingItem item = currenObjectInCollision.GetComponent<CraftingItem>();
                if (item.AreRequirementsFullfilled())
                {
                    item.BuildingStarted();
                    isButtonPressedDown = true;
                    Debug.Log("start budowy");
                }

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
