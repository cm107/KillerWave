using UnityEngine;

public class ShopPiece : MonoBehaviour
{
    [SerializeField] SOShopSelection shopSelection;
    public SOShopSelection ShopSelection
    {
        get {return shopSelection;}
        set {shopSelection = value;}
    }

    void Awake()
    {
        // icon slot
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = shopSelection.icon;
        
        // selection value
        Transform itemTextTransform = transform.Find("itemText");
        if (itemTextTransform != null)
            itemTextTransform.GetComponent<TextMesh>().text = shopSelection.cost;
    }
}
