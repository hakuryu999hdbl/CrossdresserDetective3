using UnityEngine;

public class SearchState : EnemyBaseState
{
    private float searchTimer;
    private float lookTimer;
    private bool reachedLastPos;
    private float turnTimer;

    public override void EnterState(EnemyController enemy)
    {
        enemy.checkArea.SetSearchColor();//设置视野范围黄色
        enemy.animState = 2; // 先保持跑
        searchTimer = enemy.searchTime;
        lookTimer = enemy.lookTime;
        reachedLastPos = false;
        turnTimer = 0f;

        if (enemy.questionSign != null)
            enemy.questionSign.SetActive(true);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        // 重新看到目标，立刻追击
        if (enemy.attackList.Count > 0)
        {
            if (enemy.questionSign != null)
                enemy.questionSign.SetActive(false);

            enemy.EnterBattleState();//虚类进入战斗
            return;
        }

        searchTimer -= Time.deltaTime;

        if (!reachedLastPos)
        {
            enemy.animState = 2;//没有抵达最后位置的时候保持跑

            Vector2 targetPos = new Vector2(enemy.lastKnownTargetPos.x, enemy.transform.position.y);

            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                targetPos,
                enemy.speed * Time.deltaTime
            );

            // 面向最后看到的位置
            if (enemy.transform.position.x < enemy.lastKnownTargetPos.x)
            {
                enemy.transform.localScale = new Vector3(1, 1, 1);
            }
            else 
            {
                enemy.transform.localScale = new Vector3(-1, 1, 1);
            }
               

            // 到达最后位置，开始左右看
            if (Mathf.Abs(enemy.transform.position.x - enemy.lastKnownTargetPos.x) < 0.1f)
            {
                reachedLastPos = true;
                enemy.animState = 0;

                if (enemy.questionSign != null)
                    enemy.questionSign.SetActive(true);
            }

            // 没到也超时了，开始疑惑
            if (searchTimer <= 0f)
            {
                reachedLastPos = true;
                enemy.animState = 0;

                if (enemy.questionSign != null)
                    enemy.questionSign.SetActive(true);
            }

            return;
        }

        // 左右看
        enemy.animState = 0;
        lookTimer -= Time.deltaTime;
        turnTimer -= Time.deltaTime;

        if (turnTimer <= 0f)
        {
            enemy.TurnAround();
            turnTimer = 0.45f;
        }

        if (lookTimer <= 0f)
        {
            if (enemy.questionSign != null)
                enemy.questionSign.SetActive(false);

            enemy.TransitionToState(enemy.patrolState);
        }

    }
}
