using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IActorTemplate
{
    int travelSpeed;
    int health;
    int hitPower;
    GameObject actor;
    GameObject fire;

    Vector3 direction;
    Rigidbody rb;
    public static bool mobile = false;

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

    GameObject[] screenPoints = new GameObject[2];

    float camTravelSpeed;
    public float CamTravelSpeed
    {
        get {return camTravelSpeed;}
        set {camTravelSpeed = value;}
    }
    float movingScreen;

    void Start()
    {
        // Note: Viewport space coordinates are between 0 and 1.
        Vector3 view = Camera.main.WorldToViewportPoint(new Vector3(1,1,0));
        CalculateBoundaries();

        _Player = GameObject.Find("_Player");

        mobile = false;

        #if UNITY_ANDROID && !UNITY_EDITOR
        mobile = true;
        InvokeRepeating("Attack", time: 0, repeatRate: 0.3f);
        rb = GetComponent<Rigidbody>();
        #endif
    }

    private void CalculateBoundaries()
    {
        screenPoints[0] = new GameObject("p1");
        screenPoints[1] = new GameObject("p2");
        Vector3 v1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 300));
        Vector3 v2 = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 300));
        screenPoints[0].transform.position = v1;
        screenPoints[1].transform.position = v2;
        screenPoints[0].transform.SetParent(this.transform.parent);
        screenPoints[1].transform.SetParent(this.transform.parent);
        movingScreen = screenPoints[1].transform.position.x;
    }

    void Update()
    {
        if (Time.timeScale == 1)
        {
            PlayersSpeedWithCamera();
            if (mobile)
                MobileControls();
            else
            {
                Movement();
                Attack();
            }
        }
    }

    private void MobileControls()
    {
        if (
            Input.touchCount > 0
            && EventSystem.current.currentSelectedGameObject == null
        ) // One or more fingers touching the screen.
        {
            Touch touch = Input.GetTouch(0); // Get the first location that was touched.
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(touch.position.x, touch.position.y, 300)
            );
            touchPosition.z = 0;
            direction = touchPosition - transform.position;
            rb.velocity = new Vector3(direction.x, direction.y, 0) * 5;
            direction.x += movingScreen;

            if (touch.phase == TouchPhase.Ended)
                rb.velocity = Vector3.zero;
        }
    }

    private void PlayersSpeedWithCamera()
    {
        if (camTravelSpeed > 1)
        {
            transform.position += Vector3.right * Time.deltaTime * camTravelSpeed;
            movingScreen += Time.deltaTime * camTravelSpeed;
        }
        else
            movingScreen = 0;
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
        GameObject explode = GameObject.Instantiate(Resources.Load("explode")) as GameObject;
        explode.transform.position = this.gameObject.transform.position;
        GameManager.Instance.LifeLost();
        Destroy(this.gameObject);
    }

    void Movement()
    {
        float horAxisRaw = Input.GetAxisRaw("Horizontal");
        if (horAxisRaw > 0)
        {
            if (transform.localPosition.x < (screenPoints[1].transform.localPosition.x - screenPoints[1].transform.localPosition.x / 30f) + movingScreen)
            {
                transform.localPosition += new Vector3(horAxisRaw * Time.deltaTime * travelSpeed, 0, 0);
            }
        }
        if (horAxisRaw < 0)
        {
            if (transform.localPosition.x > (screenPoints[0].transform.localPosition.x + screenPoints[0].transform.localPosition.x / 30f) + movingScreen)
            {
                transform.localPosition += new Vector3(horAxisRaw* Time.deltaTime * travelSpeed, 0, 0);
            }
        }
    
        float vertAxisRaw = Input.GetAxisRaw("Vertical");
        if (vertAxisRaw < 0)

            if (transform.localPosition.y > (screenPoints[1].transform.localPosition.y - screenPoints[1].transform.localPosition.y / 3f))
        {
            transform.localPosition += new Vector3 (0, vertAxisRaw * Time.deltaTime * travelSpeed, 0);
        }
    
        if (vertAxisRaw > 0)
            if (transform.localPosition.y < (screenPoints[0].transform.localPosition.y - screenPoints[0].transform.localPosition.y / 5f))
        {
            transform.localPosition += new Vector3 (0, vertAxisRaw * Time.deltaTime * travelSpeed, 0);
        }
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1") || mobile)
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
