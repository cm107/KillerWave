using UnityEngine;

public class PlayerBullet : MonoBehaviour, IActorTemplate
{
    GameObject actor;
    int hitPower;
    int health;
    int travelSpeed;

    [SerializeField]
    SOActorModel bulletModel;

    void Awake()
    {
        ActorStats(bulletModel);
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(travelSpeed, 0, 0) * Time.deltaTime;
    }

    public void ActorStats(SOActorModel actorModel)
    {
        hitPower = actorModel.hitPower;
        health = actorModel.health;
        travelSpeed = actorModel.speed;
        actor = actorModel.actor;
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public int SendDamage()
    {
        return hitPower;
    }

    public void TakeDamage(int incomingDamage)
    {
        health -= incomingDamage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            IActorTemplate otherActor = other.GetComponent<IActorTemplate>();
            if (otherActor != null)
            {
                if (health >= 1)
                    health -= otherActor.SendDamage();
                if (health <= 0)
                    Die();
            }
        }
    }

    void OnBecameInvisible()
    {
        // Removes bullets that have left the screen.
        Destroy(this.gameObject);
    }
}
