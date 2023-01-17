using UnityEngine;

public class Player : MonoBehaviour, IActorTemplate
{
    int travelSpeed;
    int health;
    int hitPower;
    GameObject actor;
    GameObject fire;

    public int Health
    {
        get {return health;}
        set {health = value;}
    }

    public GameObject Fire
    {
        get {return fire;}
        set {fire = value;}
    }

    GameObject _Player;

    float width;
    float height;

    void Start()
    {
        // Note: Viewport space coordinates are between 0 and 1.
        Vector3 view = Camera.main.WorldToViewportPoint(new Vector3(1,1,0));
        height = 1 / (view.y - 0.5f);
        width = 1 / (view.x - 0.5f);

        _Player = GameObject.Find("_Player");
    }

    void Update()
    {
        if (Time.timeScale == 1)
        {
            Movement();
            Attack();
        }
    }

    public void ActorStats(SOActorModel actorModel)
    {
        health = actorModel.health;
        travelSpeed = actorModel.speed;
        hitPower = actorModel.hitPower;
        fire = actorModel.actorsBullets;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (health >= 1)
            {
                // Note: This will be added in Chapter 6.
                Transform energy = transform.Find("energy +1 (Clone)");
                if (energy != null)
                {
                    Destroy(energy.gameObject);
                    health -= other.GetComponent<IActorTemplate>().SendDamage();
                }
                else
                    health -= 1;
            }

            if (health <= 0)
                Die();
        }
    }

    public int SendDamage()
    {
        return this.hitPower;
    }

    public void TakeDamage(int incomingDamage)
    {
        health -= incomingDamage;
    }

    public void Die()
    {
        GameManager.Instance.LifeLost();
        Destroy(this.gameObject);
    }

    void Movement()
    {
        float horAxisRaw = Input.GetAxisRaw("Horizontal");
        if (horAxisRaw > 0)
        {
            if (transform.localPosition.x < width + width / 0.9f)
            {
                transform.localPosition += new Vector3(
                    horAxisRaw * Time.deltaTime * travelSpeed,
                    0, 0
                );
            }
        }
        else if (horAxisRaw < 0)
        {
            if (transform.localPosition.x > width + width / 6)
            {
                transform.localPosition += new Vector3(
                    horAxisRaw * Time.deltaTime * travelSpeed,
                    0, 0
                );
            }
        }

        float vertAxisRaw = Input.GetAxisRaw("Vertical");
        if (vertAxisRaw < 0)
        {
            if (transform.localPosition.y > -height / 3f)
            {
                transform.localPosition += new Vector3(
                    0,
                    vertAxisRaw * Time.deltaTime * travelSpeed,
                    0
                );
            }
        }
        if (vertAxisRaw > 0)
        {
            if (transform.localPosition.y < height / 2.5f)
            {
                transform.localPosition += new Vector3(
                    0,
                    vertAxisRaw * Time.deltaTime * travelSpeed,
                    0
                );
            }
        }
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (fire == null)
                Debug.Log("fire is null");
            GameObject bullet = GameObject.Instantiate(
                original: fire,
                position: transform.position,
                rotation: Quaternion.Euler(Vector3.zero)
            ) as GameObject;
            bullet.transform.SetParent(_Player.transform);
            bullet.transform.localScale = new Vector3(7,7,7);
        }
    }
}
