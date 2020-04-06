using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject pressETextObject;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private Movement movement;

    [Title("Sounds")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;

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
            Debug.Log("Added object: " + other.gameObject);

            objectsInCollisionList.Add(other.gameObject);
            currenObjectInCollision = getNearestCollidesObject();
            if(!holdingPickup) ShowText(true);
        }
        else if (other.CompareTag("CraftingItem"))
        {
            var craftingItem = other.GetComponent<CraftingItem>();

            if (craftingItem.AlreadyCompleted) return;
            
            objectsInCollisionList.Add(other.gameObject);
            Debug.Log("Added object: " + other.gameObject);
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
            if (objectsInCollisionList.Contains(other.gameObject)) objectsInCollisionList.Remove(other.gameObject);
            
            Debug.Log("Removed: " + other.gameObject);
            string s = "";
            objectsInCollisionList.ForEach(arg => s += (arg.name) + " | ");
            Debug.Log("Elements: " + s);

            currenObjectInCollision = getNearestCollidesObject();
            if(other.CompareTag("Pickup")) other.GetComponent<Pickup>().ChangeHighlight(false);
        }
    }


    private void Update()
    {
        // Debug.Log("HoldingItem: " + holdigPickup + " HoldingCraftinItem: " + holdingCraftinItem);
        // Debug.Log("currObjInCol: " + currenObjectInCollision + " List: " + objectsInCollisionList.Count);
        if (Input.GetKeyUp(KeyCode.E) && currenObjectInCollision)
        {
            var craftingItem = currenObjectInCollision.GetComponent<CraftingItem>();


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
                    craftingItem.BuildingFinished();
                    movement.EnableMovement();
                    OnTriggerExit(currenObjectInCollision.GetComponent<Collider>());
                    currenObjectInCollision = null;
                   ShowText(false);
                Debug.Log("stop budowy");
                
                }
            }
            else // is Holding item
            {
                if(currenObjectInCollision.tag == "CraftingItem")
                {
                    if(craftingItem.PutItem(holdingPickup))
                    {
                        Debug.Log("udało się wsadzić item");
                        
                        if (craftingItem.AlreadyCompleted)
                        {
                            if (objectsInCollisionList.Contains(craftingItem.gameObject))
                            {
                                Debug.Log("Removing: " + craftingItem.gameObject);
                                objectsInCollisionList.Remove(craftingItem.gameObject);
                            }
                        }

                        
                        SoundManager.PlaySound(correctSound);
                        holdingPickup.gameObject.transform.SetParent(currenObjectInCollision.transform);
                        holdingPickup.gameObject.SetActive(false);
                        DropItem(true);
                        holdingPickup = null;
                    }
                    else
                    {
                        SoundManager.PlaySound(wrongSound);
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
                    if (objectsInCollisionList.Contains(item.gameObject))
                    {
                        Debug.Log("Removing: " + item.gameObject);
                        objectsInCollisionList.Remove(item.gameObject);
                    }
                    // movement.DisableMovement();
                    isButtonPressedDown = false;
                    currenObjectInCollision = null;
                    ShowText(false);
                    //  Debug.Log("start budowy");
                }

            }
        }
        
        if (!currenObjectInCollision) ShowText(false);
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
