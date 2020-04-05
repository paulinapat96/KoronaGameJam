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
    private Renderer renderer;

    [Title("Local refs")]
    [SerializeField] private Collider _Collider;
    public Sprite IconEnabled => _IconEnabled;
    public Sprite IconDisabled => _IconDisabled;
    public string ItemName => _ItemName;
    public Collider Collider => _Collider;


    private void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        Material newMaterial = renderer.material;
        newMaterial.EnableKeyword("_EMISSION");
        renderer.material = newMaterial;
        renderer.gameObject.SetActive(false);
        renderer.gameObject.SetActive(true);
        ChangeHighlight(false);


    }
    private void OnDestroy()
    {
        //make some magic
    }

    public void ChangeHighlight(bool isActive)
    {
        if(isActive)
        {
            renderer.sharedMaterials[0].SetColor("_EmissionColor", new Color(0.25f, 0.25f, 0.25f, 1.0F));
        }
        else
        {
            renderer.sharedMaterials[0].SetColor("_EmissionColor", new Color(0, 0, 0, 1.0F));
        }
    }
}
