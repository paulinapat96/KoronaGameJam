using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class RequiredItem : MonoBehaviour
{
    [Title("Local refs")]
    [SerializeField] private Image _Ico;
    [SerializeField] private Image _AlreadyBrought;

    [Title("Sprites")] 
    [SerializeField] private Sprite _NotBrought;
    [SerializeField] private Sprite _Brought;
    
    public void SetSprite(Sprite sprite, bool hasBeenBrought)
    {
        _Ico.sprite = sprite;

        _AlreadyBrought.sprite = hasBeenBrought ? _Brought : _NotBrought;
    }
}
