using Sirenix.Utilities;
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
    [SerializeField] private GameObject playerCanvas;
    List<GameObject> objectsInCollision;
    bool isCanvasActive = false;

    private void Start()
    {
        objectsInCollision = new List<GameObject>();
        ShowText(isCanvasActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {   
            objectsInCollision.Add(other.gameObject);
            currenObjectInCollision = getNearestCollidesObject();
            ShowText(true);
        }
        else if (other.CompareTag("CraftingItem"))
        {
            objectsInCollision.Add(other.gameObject);
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
            objectsInCollision.Remove(other.gameObject);
            currenObjectInCollision = getNearestCollidesObject();
        }
    }

    private void Update()
    {
        Debug.Log("Holding item:" + holdigPickup);
        Debug.Log("Current collision:" + currenObjectInCollision + " List: "+objectsInCollision.Count);
        
        if (Input.GetKeyUp(KeyCode.E) && currenObjectInCollision)
        {
            currenObjectInCollision = getNearestCollidesObject();
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
       if(isCanvasActive) playerCanvas.transform.rotation = Quaternion.Euler(0, -transform.rotation.y, 0);
    }

    private void PickItem()
    {
        // TODO: Play animator - podniesienie rąk
        Debug.Log("pick");
        holdigPickup.transform.SetParent(transform);
        holdigPickup.transform.localPosition = new Vector3(0, 0, 1f);
        ShowText(false);
    }

    private void DropItem()
    {
        // TODO: Play animator - opuszczenie rąk
        Debug.Log("drop");
        OnTriggerExit(holdigPickup.Collider);
        OnTriggerEnter(holdigPickup.Collider);
        holdigPickup.transform.SetParent(null);
        holdigPickup = null;
    }

    private void HoldingCraftingItem(GameObject item, float time)
    {
        Debug.Log(item.name + time);
    }

    private GameObject getNearestCollidesObject()
    {
        if (objectsInCollision.IsNullOrEmpty()) return null;

        GameObject objWithMinDistance = objectsInCollision[0];
        if(objectsInCollision.Count > 1)
        { 
            float minDis = Vector3.Distance(transform.position, objectsInCollision[0].transform.position);
            foreach (GameObject obj in objectsInCollision)
            {
                if(obj.CompareTag("CraftingItem"))
                {
                    if (obj.GetComponent<CraftingItem>().AreRequirementsFullfilled()) return obj;
                }
                if (Vector3.Distance(transform.position, obj.transform.position) < minDis) objWithMinDistance = obj;
            }
        }
        return objWithMinDistance;
    }

    public void OnChangeActiveItem()
    {
        
        // TODO: wyświetlenie aktywnego itemu
        ///   holdingItem.GetComponent<Pickup>().iconDisabled;
    }

    public void ShowText(bool displayText)
    {
        pressETextObject.SetActive(displayText);
        isCanvasActive = displayText;
    }
}
