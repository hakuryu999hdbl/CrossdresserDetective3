using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEncounterController : MonoBehaviour
{
    [Header("敌人生成")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public Transform[] spawnPoints;   // 生成点数组


    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool areaActivated = false;//是否被触发一遍


    private void Start()
    {
        GameManager.instance.IsEnemyCreator(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (areaActivated)
            return;

        PlayerController player =
            collision.GetComponentInParent<PlayerController>();

        if (player == null)
            return;
        ActivateArea();
        //Invoke(nameof(ActivateArea), 0.5f);//有些场景直接出来会碰到来不及触发

    }

    void ActivateArea()
    {
        areaActivated = true;

        // 生成敌人
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(0.5f, 1.5f);
            Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, -1.5f, -1);//目标是Z在0，但是门都是0.95不能更低，Y是门刚好让敌人不产生下落动画高度

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            aliveEnemies.Add(enemy);


            //记录为非场景单个敌人，区域生成敌人
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.isAreaSpawnedEnemy = true;
            }



            if (spawnPoint.GetComponent<Animator>()!=null) 
            {
                spawnPoint.GetComponent<Animator>().SetTrigger("Open");//开门动画
            }
           
        }

        // 4. 监听敌人是否全部死亡
        StartCoroutine(CheckEnemiesDead());
    }

    System.Collections.IEnumerator CheckEnemiesDead()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            aliveEnemies.RemoveAll(e =>
            {
                if (e == null) return true;

                EnemyController enemy = e.GetComponent<EnemyController>();
                if (enemy != null && enemy.isDead) return true;

                return false;
            });

            if (aliveEnemies.Count == 0)
            {
                GameManager.instance.EnemyCleanOver(this);

                //Destroy(gameObject);
                yield break;
            }
        }
    }
}
