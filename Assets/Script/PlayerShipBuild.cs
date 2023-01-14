using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class PlayerShipBuild : MonoBehaviour, IUnityAdsListener, IUnityAdsInitializationListener
{
    [SerializeField] GameObject[] shopButtons;
    GameObject target;
    GameObject tmpSelection;
    GameObject textBoxPanel;

    [SerializeField] GameObject[] visualWeapons;
    [SerializeField] SOActorModel defaultPlayerShip;
    GameObject playerShip;
    GameObject buyButton;
    GameObject bankObj;
    int bank = 600;
    bool purchaseMade = false;

    [SerializeField] string androidGameId;
    [SerializeField] string iOSGameId;
    [SerializeField] bool testMode = true;
    string adId = null;

    void Awake()
    {
        CheckPlatform();
    }

    private void CheckPlatform()
    {
        string gameId = null;
        #if UNITY_IOS
        {
            gameId = iOSGameId;
            adId = "Rewarded_iOS";
        }
        #elif UNITY_ANDROID
        {
            gameId = androidGameId;
            adId = "Rewarded_Android";
        }
        #endif
        Advertisement.Initialize(gameId, testMode, false, this);
    }

    void Start()
    {
        textBoxPanel = GameObject.Find("textBoxPanel");
        TurnOffSelectionHighlights();

        purchaseMade = false;
        bankObj = GameObject.Find("bank");
        bankObj.GetComponentInChildren<TextMesh>().text = bank.ToString();
        buyButton = textBoxPanel.transform.Find("BUY ?").gameObject;

        TurnOffPlayerShipVisuals();
        PreparePlayerShipForUpgrade();

        StartCoroutine(WaitForAd()); // Wait until advert is initialized.
    }

    private IEnumerator WaitForAd()
    {
        while (!Advertisement.isInitialized)
            yield return null;
        
        LoadAd();
    }

    private void LoadAd()
    {
        Advertisement.AddListener(this);
        Advertisement.Load(adId);
    }

    private void PreparePlayerShipForUpgrade()
    {
        playerShip = GameObject.Instantiate(defaultPlayerShip.actor);

        // Disable and put off to the side while in the shop scene.
        playerShip.GetComponent<Player>().enabled = false;
        playerShip.transform.position = new Vector3(0,10000,0);
        playerShip.GetComponent<IActorTemplate>().ActorStats(defaultPlayerShip);
    }

    private void TurnOffPlayerShipVisuals()
    {
        for (int i = 0; i < visualWeapons.Length; i++)
            visualWeapons[i].gameObject.SetActive(false);
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

                    // Not already sold
                    TextMesh itemTextMesh = target.transform.Find("itemText").GetComponent<TextMesh>();
                    if (itemTextMesh.text != "SOLD")
                    {
                        // can afford
                        Affordable();

                        // can not afford
                        LackOfCredits();
                    }
                    else if (itemTextMesh.text == "SOLD")
                    {
                        SoldOut();
                    }
                }
                else if (target.name == "WATCH AD")
                {
                    WatchAdvert();
                }
                else if (target.name == "BUY ?")
                {
                    BuyItem();
                }
                else if (target.name == "START")
                {
                    StartGame();
                }
            }
        }
    }

    private void StartGame()
    {
        if (purchaseMade)
        {
            playerShip.name = "UpgradedShip";
            if (playerShip.transform.Find("energy +1(Clone)"))
                playerShip.GetComponent<Player>().Health = 2;
            DontDestroyOnLoad(playerShip);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("testLevel");
    }

    private void BuyItem()
    {
        Debug.Log("PURCHASED");
        purchaseMade = true;
        buyButton.SetActive(false);
        tmpSelection.SetActive(false);

        SOShopSelection shopSelection = tmpSelection.transform.parent.gameObject.GetComponent<ShopPiece>().ShopSelection;
        for (int i = 0; i < visualWeapons.Length; i++)
        {
            GameObject visualWeapon = visualWeapons[i];
            if (visualWeapon.name == shopSelection.iconName)
                visualWeapon.SetActive(true);
        }

        UpgradeToShip(shopSelection.iconName);
        bank = bank - System.Int32.Parse(shopSelection.cost);
        bankObj.transform.Find("bankText").GetComponent<TextMesh>().text = bank.ToString();
        tmpSelection.transform.parent.transform.Find("itemText").GetComponent<TextMesh>().text = "SOLD";
    }

    private void UpgradeToShip(string upgrade)
    {
        GameObject shipItem = GameObject.Instantiate(Resources.Load(upgrade)) as GameObject;
        shipItem.transform.SetParent(playerShip.transform);
        shipItem.transform.localPosition = Vector3.zero;
    }

    private void WatchAdvert()
    {
        Advertisement.Show(adId);
    }

    private void SoldOut()
    {
        Debug.Log("SOLD OUT");
    }

    private void LackOfCredits()
    {
        TextMesh itemTextMesh = target.transform.Find("itemText").GetComponent<TextMesh>();
        string itemText = itemTextMesh.text; // Why not do it the same way as in Affordable?
        if (bank < System.Int32.Parse(itemText))
        {
            Debug.Log("CAN'T BUY");
        }
    }

    private void Affordable()
    {
        string cost = target.transform.GetComponent<ShopPiece>().ShopSelection.cost;
        if (bank >= System.Int32.Parse(cost))
        {
            Debug.Log("CAN BUY");
            buyButton.SetActive(true);
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

    public void OnUnityAdsReady(string placementId)
    {
        // When a Unity advert is ready to play.
        // throw new NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
        // An error happened with loading the advert.
        // throw new NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // The advert has started playing.
        // throw new NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // The advert has finished playing.
        if (showResult == ShowResult.Finished)
        {
            // Reward Player
            Debug.Log("Unity Ads Rewarded Ad Completed");
            bank += 300;
            bankObj.GetComponentInChildren<TextMesh>().text = bank.ToString();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do Not Reward Player
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
        Advertisement.Load(placementId); // loads another advert using the same reward id
        TurnOffSelectionHighlights(); // deselects all buttons in the shop
    }

    public void OnInitializationComplete()
    {
        // This method will run when our advert service has been initialized.
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        // If the advert service initialization has failed.
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
