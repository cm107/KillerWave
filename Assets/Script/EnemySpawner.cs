using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] SOActorModel actorModel;
    [SerializeField] float spawnRate;
    [SerializeField, Range(0, 10)] int quantity;
    GameObject enemies;

    void Awake()
    {
        enemies = GameObject.Find("_Enemies");
        StartCoroutine(FireEnemy(quantity, spawnRate));
    }

    private IEnumerator FireEnemy(int quantity, float spawnRate)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject enemyUnit = CreateEnemy();
            enemyUnit.gameObject.transform.SetParent(this.transform);
            enemyUnit.transform.position = transform.position;
            yield return new WaitForSeconds(spawnRate);
        }
        yield return null;
    }

    private GameObject CreateEnemy()
    {
        GameObject enemy = GameObject.Instantiate(actorModel.actor) as GameObject;
        IActorTemplate actor = enemy.GetComponent<IActorTemplate>();
        actor.ActorStats(actorModel);
        enemy.name = actorModel.actorName.ToString();
        return enemy;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
