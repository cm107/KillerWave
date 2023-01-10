using System;
using UnityEngine;

public class PlayerShipBuild : MonoBehaviour
{
    [SerializeField] GameObject[] shopButtons;
    GameObject target;
    GameObject tmpSelection;
    GameObject textBoxPanel;

    void Start()
    {
        textBoxPanel = GameObject.Find("textBoxPanel");
        TurnOffSelectionHighlights();
    }

    void Update()
    {
        AttemptSelection();
    }

    void AttemptSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);

            if (target != null)
            {
                if (target.transform.Find("itemText"))
                {
                    TurnOffSelectionHighlights();
                    Select();
                    UpdateDescriptionBox();
                }
            }
        }
    }

    private void UpdateDescriptionBox()
    {
        SOShopSelection shopSelection = tmpSelection.GetComponentInParent<ShopPiece>().ShopSelection;
        TextMesh name = textBoxPanel.transform.Find("name").GetComponent<TextMesh>();
        name.text = shopSelection.iconName;
        TextMesh desc = textBoxPanel.transform.Find("desc").GetComponent<TextMesh>();
        desc.text = shopSelection.description;
    }

    private void Select()
    {
        tmpSelection = target.transform.Find("SelectionQuad").gameObject;
        tmpSelection.SetActive(true);
    }

    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 100, out hit))
            target = hit.collider.gameObject;
        return target;
    }

    void TurnOffSelectionHighlights()
    {
        for (int i = 0; i < shopButtons.Length; i++)
            shopButtons[i].SetActive(false);
    }
}
