using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    SOActorModel actorModel;
    GameObject playerShip;

    void Start()
    {
        CreatePlayer();
    }

    void CreatePlayer()
    {
        // Create Player
        actorModel = Object.Instantiate(
            Resources.Load("PLayer_Default")
        ) as SOActorModel;
        playerShip = GameObject.Instantiate(
            actorModel.actor
        ) as GameObject;
        Player player = playerShip.GetComponent<Player>();
        player.ActorStats(actorModel);

        // Set player up
        playerShip.transform.rotation = Quaternion.Euler(0, 180, 0);
        playerShip.transform.localScale = new Vector3(60, 60, 60);
        playerShip.GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3(25, 25, 25);
        playerShip.name = "Player"; // Get rid of the (Clone) part added by Unity.
        playerShip.transform.SetParent(this.transform);
        playerShip.transform.position = Vector3.zero;
    }

    void Update()
    {
        
    }
}
