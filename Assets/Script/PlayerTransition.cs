using UnityEngine;
using System.Collections;
using System;

public class PlayerTransition : MonoBehaviour
{
    Vector3 transitionToEnd = new Vector3(-100, 0, 0); // TODO: Rename to transitionToStart?
    Vector3 transitionToCompleteGame = new Vector3(7000, 0, 0);
    Vector3 readyPos = new Vector3(900, 0, 0);
    Vector3 startPos;
    float distCovered;
    float journeyLength;

    bool levelStarted = true; // false after the transition of player's animation has finished
    bool speedOff = false; // true when the ship leaves the level
    bool levelEnds = false; // true when level ends and ship moves to exit position
    bool gameCompleted = false; // when game complete

    public bool LevelEnds
    {
        get {return levelEnds;}
        set {levelEnds = value;}
    }
    public bool GameCompleted
    {
        get {return gameCompleted;}
        set {gameCompleted = value;}
    }

    void Start()
    {
        this.transform.localPosition = Vector3.zero;
        startPos = transform.position;
        Distance();
    }

    private void Distance()
    {
        journeyLength = Vector3.Distance(startPos, readyPos);
    }

    void Update()
    {
        if (levelStarted)
            PlayerMovement(transitionToEnd, 10);
        if (levelEnds)
        {
            GetComponent<Player>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            Distance();
            PlayerMovement(transitionToEnd, 200);
        }
        if (gameCompleted)
        {
            GetComponent<Player>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            PlayerMovement(transitionToCompleteGame, 200);
        }
        if (speedOff)
            Invoke("SpeedOff", 1f);
    }

    private void PlayerMovement(Vector3 point, float transitionSpeed)
    {
        if (
            Mathf.Round(transform.localPosition.x) >= readyPos.x - 5
            && Mathf.Round(transform.localPosition.x) <= readyPos.x + 5
            && Mathf.Round(transform.localPosition.y) >= -5f
            && Mathf.Round(transform.localPosition.y) <= 5f
        )
        {
            if (levelEnds)
            {
                levelEnds = false;
                speedOff = true;
            }
            if (levelStarted)
            {
                levelStarted = false;
                distCovered = 0;
                GetComponent<Player>().enabled = true;
            }
        }
        else
        {
            distCovered += Time.deltaTime * transitionSpeed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, point, fractionOfJourney);
        }
    }

    void SpeedOff()
    {
        transform.Translate(Vector3.left * Time.deltaTime * 800);
    }
}
