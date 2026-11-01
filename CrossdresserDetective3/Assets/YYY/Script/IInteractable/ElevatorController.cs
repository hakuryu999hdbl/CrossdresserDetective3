using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour, IInteractable
{
    [Header("移动点位")]
    public Transform topPoint;
    public Transform bottomPoint;

    [Header("移动设置")]
    public float moveSpeed = 3f;
    public bool startAtBottom = true;

    [Header("提示")]
    public GameObject upEffect;
    public GameObject downEffect;

    private bool isMoving;
    private bool isAtTop;

    public Rigidbody2D rb;

 


    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        isAtTop = !startAtBottom;

        Vector2 startPos = startAtBottom ? bottomPoint.position : topPoint.position;
        transform.position = startPos;

        CloseAllDoors();
        OpenCurrentDoor();

        RefreshEffect();

        if (obstacle != null)
            obstacle.SetActive(false);//隐藏透明挡板
    }

    public void TriggerAction()
    {
        if (isMoving)
        {
            ReverseElevator();
            return;
        }


        Debug.Log("电梯移动");

        currentTarget = isAtTop ? bottomPoint : topPoint;
        moveCoroutine = StartCoroutine(MoveTo(currentTarget));

        CloseAllDoors();//电梯开始运行后就全部关上门
    }

    private IEnumerator MoveTo(Transform target)
    {
        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Elevator_1);
        isMoving = true;

        RefreshObstacle();//更新透明挡板
        RefreshFloorDirection();//更新按钮图标

        if (upEffect != null) upEffect.SetActive(false);
        if (downEffect != null) downEffect.SetActive(false);

        Vector3 targetPos = target.position;

        while (Vector3.Distance(transform.position, targetPos) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPos;

        isAtTop = target == topPoint;
        isMoving = false;
        moveCoroutine = null;

        OpenCurrentDoor();//到位置了门打开


        RefreshEffect();

        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Elevator_2);

        RefreshObstacle();//更新透明挡板
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            collision.GetComponent<PlayerController>().isOnElevator = true;

            RefreshEffect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {

            collision.GetComponent<PlayerController>().isOnElevator = false;

            RefreshEffect();
        }
    }

    private void RefreshEffect()
    {
        if (upEffect != null)
            upEffect.SetActive(!isAtTop);   // 不在顶层才能上去

        if (downEffect != null)
            downEffect.SetActive(isAtTop);  // 在顶层才能下去

        RefreshFloorDirection();
    }
    private void RefreshFloorDirection()
    {
        bool goingUp = isMoving
            ? currentTarget == topPoint
            : isAtTop;

        if (topDoor != null)
            topDoor.SetDirection(goingUp);

        if (bottomDoor != null)
            bottomDoor.SetDirection(goingUp);
    }


    //电梯门关上
    public ElevatorFloorDoor topDoor;
    public ElevatorFloorDoor bottomDoor;

    private void CloseAllDoors()
    {
        topDoor.Close();
        bottomDoor.Close();

        
    }

    private void OpenCurrentDoor()
    {
        if (isAtTop)
            topDoor.Open();
        else
            bottomDoor.Open();

        
    }



    //开一半回去
    private Coroutine moveCoroutine;
    private Transform currentTarget;
    private void ReverseElevator()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        currentTarget = currentTarget == topPoint ? bottomPoint : topPoint;

        RefreshObstacle();

        moveCoroutine = StartCoroutine(MoveTo(currentTarget));
    }


    //按钮的位置叫上来
    public void CallToTop()
    {
        if (isMoving) return;
        if (isAtTop) return;

        currentTarget = topPoint;
        moveCoroutine = StartCoroutine(MoveTo(currentTarget));
        CloseAllDoors();
    }

    public void CallToBottom()
    {
        if (isMoving) return;
        if (!isAtTop) return;

        currentTarget = bottomPoint;
        moveCoroutine = StartCoroutine(MoveTo(currentTarget));
        CloseAllDoors();
    }






    [Header("电梯下降挡板")]
    public GameObject obstacle;

    private void RefreshObstacle()
    {
        if (obstacle == null) return;

        // 只有正在向下移动时才开启
        bool shouldBlock = isMoving && currentTarget == bottomPoint;

        obstacle.SetActive(shouldBlock);
    }
}