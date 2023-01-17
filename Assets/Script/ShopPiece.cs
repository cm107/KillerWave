using UnityEngine;
using UnityEngine.UI;

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
        Image itemImage = transform.GetChild(3).GetComponent<Image>();
        if (itemImage != null)
            itemImage.sprite = shopSelection.icon;
        if (transform.Find("itemText"))
            GetComponentInChildren<Text>().text = shopSelection.cost.ToString();
    }
}
