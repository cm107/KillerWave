using System;
using UnityEngine;

public class EnemyWave : MonoBehaviour, IActorTemplate
{
    int health;
    int travelSpeed;
    int fireSpeed;
    int hitPower;
    int score;

    // wave enemy
    [SerializeField] float verticalSpeed = 2;
    [SerializeField] float verticalAmplitude = 1;
    Vector3 sineVer;
    float time;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Note: This doesn't get executed when timeScale is 0.
        //       Update, on the other hand, does.
        Attack();
    }

    private void Attack()
    {
        time += Time.deltaTime;
        sineVer.y = Mathf.Sin(time * verticalSpeed) * verticalAmplitude;
        transform.position = new Vector3(
            transform.position.x + travelSpeed * Time.deltaTime,
            transform.position.y + sineVer.y,
            transform.position.z
        );
    }

    public void ActorStats(SOActorModel actorModel)
    {
        health = actorModel.health;
        travelSpeed = actorModel.speed;
        hitPower = actorModel.hitPower;
        score = actorModel.score;
    }

    public void Die()
    {
        GameObject explode = GameObject.Instantiate(Resources.Load("explode")) as GameObject;
        explode.transform.position = this.gameObject.transform.position;
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
        // if the player or their bullet hits you
        if (other.tag == "Player")
        {
            if (health >= 1)
            {
                IActorTemplate otherActor = other.GetComponent<IActorTemplate>();
                health -= otherActor.SendDamage();
            }
            if (health <= 0)
            {
                GameManager.Instance.GetComponent<ScoreManager>().SetScore(score);
                Die();
            }
        }
    }
}
