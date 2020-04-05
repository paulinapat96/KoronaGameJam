using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject pressETextObject;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private Movement movement;
    
    Pickup holdingPickup = null;
    GameObject holdingCraftinItem = null;
    private GameObject currenObjectInCollision = null;
    List<GameObject> objectsInCollisionList;
    private bool isButtonPressedDown = false;
    private bool isCanvasActive = false;

    private void Start()
    {
        objectsInCollisionList = new List<GameObject>();
        ShowText(isCanvasActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {   
            objectsInCollisionList.Add(other.gameObject);
            currenObjectInCollision = getNearestCollidesObject();
            if(!holdingPickup) ShowText(true);
        }
        else if (other.CompareTag("CraftingItem"))
        {
            objectsInCollisionList.Add(other.gameObject);
            var craftingItem = other.GetComponent<CraftingItem>();
            currenObjectInCollision = other.gameObject;
            
            if ((craftingItem.IsUnlocked && craftingItem.AreRequirementsFullfilled()) || (holdingPickup && craftingItem.CanPutItem(holdingPickup)))/////////
            {
                ShowText(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup") || other.CompareTag("CraftingItem"))
        {
            pressETextObject.SetActive(false);
            objectsInCollisionList.Remove(other.gameObject);
            currenObjectInCollision = getNearestCollidesObject();
            if(other.CompareTag("Pickup")) other.GetComponent<Pickup>().ChangeHighlight(false);
        }
    }


    private void Update()
    {
        // Debug.Log("HoldingItem: " + holdigPickup + " HoldingCraftinItem: " + holdingCraftinItem);
        if (Input.GetKeyUp(KeyCode.E) && currenObjectInCollision)
        {

            Debug.Log("currObjInCol: " + currenObjectInCollision + " List: " + objectsInCollisionList.Count);
            //currenObjectInCollision = getNearestCollidesObject();
            if (!holdingPickup)
            {
                
                if (currenObjectInCollision.tag == "Pickup")
                {
                    holdingPickup = currenObjectInCollision.GetComponent<Pickup>();
                    PickItem();
                }

                if (currenObjectInCollision.tag == "CraftingItem" && isButtonPressedDown)
                {
                    currenObjectInCollision.GetComponent<CraftingItem>().BuildingFinished();
                    movement.EnableMovement();
                //  Debug.Log("stop budowy");

                }
            }
            else
            {
                if(currenObjectInCollision.tag == "CraftingItem")
                {
                    if(currenObjectInCollision.GetComponent<CraftingItem>().PutItem(holdingPickup))
                    {
                        Debug.Log("udało się wsadzić item");
                        holdingPickup.gameObject.transform.SetParent(currenObjectInCollision.transform);
                        holdingPickup.gameObject.SetActive(false);
                        DropItem(true);
                        holdingPickup = null;
                    }
                    else
                    {
                  //     Debug.Log("nie mozesz wsadzić itemu");
                    }    
                }

                if(holdingPickup) DropItem(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currenObjectInCollision && currenObjectInCollision.CompareTag("CraftingItem") && !holdingPickup)
            {  
                CraftingItem item = currenObjectInCollision.GetComponent<CraftingItem>();
                if (item.AreRequirementsFullfilled())
                {
                    item.BuildingStarted();
                    movement.DisableMovement();
                    isButtonPressedDown = true;
                  //  Debug.Log("start budowy");
                }

            }

        }
        if(isCanvasActive) playerCanvas.transform.rotation = Quaternion.Euler(0, -transform.rotation.y, 0);
    }

    private void PickItem()
    {
      //  Debug.Log("pick");
        movement.PlayerHoldItem();
        holdingPickup.transform.SetParent(transform);
        holdingPickup.transform.localPosition = holdingPickup.HoldingItemPosition;
        holdingPickup.GetComponent<Pickup>().ChangeHighlight(false);
        ShowText(false);
    }

    private void DropItem(bool isItemDestroing)
    {
       // Debug.Log("drop");
        movement.PlayerReleasedItem();
        OnTriggerExit(holdingPickup.Collider);
        Pickup temp = holdingPickup;
        holdingPickup.transform.SetParent(null);
        holdingPickup = null;
        if (!isItemDestroing) OnTriggerEnter(temp.Collider);
        else ShowText(true);

    }

    private GameObject getNearestCollidesObject()
    {
        if (objectsInCollisionList.IsNullOrEmpty()) return null;

        GameObject objWithMinDistance = objectsInCollisionList[0];
        if(objectsInCollisionList.Count > 1)
        { 
            float minDis = Vector3.Distance(transform.position, objectsInCollisionList[0].transform.position);
            foreach (GameObject obj in objectsInCollisionList)
            {
                if(obj.CompareTag("CraftingItem"))
                {
                    if (obj.GetComponent<CraftingItem>().AreRequirementsFullfilled()) return obj;
                }
                if (Vector3.Distance(transform.position, obj.transform.position) < minDis) objWithMinDistance = obj;
            }
        }
        if(objWithMinDistance.CompareTag("Pickup") && !holdingPickup) objWithMinDistance.GetComponent<Pickup>().ChangeHighlight(true);
        return objWithMinDistance;
    }

    public void ShowText(bool displayText)
    {
        pressETextObject.SetActive(displayText);
        isCanvasActive = displayText;
    }
}
