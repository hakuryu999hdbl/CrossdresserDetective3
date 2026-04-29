using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        //Debug.Log("发现敌人！！！！");
        enemy.animState = 2;

        if (enemy.attackList == null || enemy.attackList.Count <= 0)
        {
            enemy.targetPoint = null;
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        enemy.targetPoint = enemy.attackList[0];//列表第一个作为追踪目标（最近的目标）
    }

    public override void OnUpdate(EnemyController enemy)
    {
        // 有炸弹时不再追踪攻击，直接回巡逻
        if (enemy.hasBomb)
        {
            enemy.attackList.Clear();
            enemy.targetPoint = null;
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        // 清理已经不存在、被销毁、或不再是可检测层的目标
        enemy.attackList.RemoveAll(t =>
            t == null ||
            (!t.CompareTag("Player") && !t.CompareTag("Bomb")) ||
            (t.CompareTag("Bomb") && t.gameObject.layer != LayerMask.NameToLayer("Bomb"))
        );




        if (enemy.attackList.Count <= 0)
        {
            enemy.targetPoint = null;
            enemy.TransitionToState(enemy.patrolState);//一旦列表为0恢复巡逻
            return;
        }


        //出现多个目标的时候，最近的优先
        if (enemy.attackList.Count > 1)
        {

            for (int i = 0; i < enemy.attackList.Count; i++)
            {
                if (Mathf.Abs(enemy.transform.position.x - enemy.attackList[i].position.x) < Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x))
                {

                    enemy.targetPoint = enemy.attackList[i];

                }//距离当前目标之间的每一个目标的X差距绝对值

            }

        }
        else if (enemy.attackList.Count == 1)//如果列表里就一个目标，那就追踪那个
        {
            enemy.targetPoint = enemy.attackList[0];
        }


        //根据tag来判断不同动作

        if(enemy.targetPoint != null)
        {
            if (enemy.targetPoint.CompareTag("Player"))
                enemy.AttackAction();
            if (enemy.targetPoint.CompareTag("Bomb"))
                enemy.SkillAction();

            enemy.MoveToTarget();
        }
        else
        {
            enemy.TransitionToState(enemy.patrolState);
        }
     

       

      
    }
}
