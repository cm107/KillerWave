using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    SOActorModel actorModel;
    GameObject playerShip;
    bool upgradedShip = false;

    void Start()
    {
        CreatePlayer();
        GetComponentInChildren<Player>().enabled = true; // re-enable (when leaving shop scene and entering testLevel scene)
        GameManager.Instance.CameraSetup(); // temporary fix
    }

    void CreatePlayer()
    {
        // been shopping
        if (GameObject.Find("UpgradedShip"))
            upgradedShip = true;
        
        // not shopped or died
        if (!upgradedShip || GameManager.Instance.Died)
        {
            GameManager.Instance.Died = false;
            actorModel = Object.Instantiate(
                Resources.Load("Player_Default")
            ) as SOActorModel;
            playerShip = GameObject.Instantiate(
                actorModel.actor,
                this.transform.position,
                Quaternion.Euler(270, 180, 0)
            ) as GameObject;
            playerShip.GetComponent<IActorTemplate>().ActorStats(actorModel);
        }
        else
        {
            playerShip = GameObject.Find("UpgradedShip");
        }

        // // Create Player
        // actorModel = Object.Instantiate(
        //     Resources.Load("PLayer_Default")
        // ) as SOActorModel;
        // playerShip = GameObject.Instantiate(
        //     actorModel.actor
        // ) as GameObject;
        // Player player = playerShip.GetComponent<Player>();
        // player.ActorStats(actorModel);

        // Set player up
        playerShip.transform.rotation = Quaternion.Euler(0, 180, 0);
        playerShip.transform.localScale = new Vector3(60, 60, 60);
        playerShip.GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3(25, 25, 25);
        playerShip.name = "Player"; // Get rid of the (Clone) part added by Unity.
        playerShip.transform.SetParent(this.transform);
        playerShip.transform.position = Vector3.zero;
        GameManager.Instance.CameraSetup();
    }

    void Update()
    {
        
    }
}
