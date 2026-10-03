using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("汽车预设体（根物体挂 BackgroundCar）")]
    public BackgroundCar[] carPrefabs;

    [Header("接收器")]
    public Transform receiver;

    [Header("随机生成间隔（秒）")]
    public float minInterval = 5f;
    public float maxInterval = 12f;

    [Header("随机移动速度")]
    public float minSpeed = 3f;
    public float maxSpeed = 5f;

    private Coroutine spawnRoutine;

    [Header("生成位置 Z 随机偏移")]
    public float minZOffset = -0.1f;
    public float maxZOffset = 0.1f;


    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minInterval, maxInterval));

            if (receiver == null ||
                carPrefabs == null ||
                carPrefabs.Length == 0)
                continue;

            BackgroundCar prefab =
                carPrefabs[Random.Range(0, carPrefabs.Length)];

            if (prefab == null)
                continue;

            Vector3 spawnPosition = transform.position;

            // 每种车单独调整高度
            spawnPosition.y += prefab.spawnYOffset;

            // 保留之前的随机 Z
            spawnPosition.z += Random.Range(minZOffset, maxZOffset);

            BackgroundCar car = Instantiate(
                prefab, spawnPosition, prefab.transform.rotation);

            car.Initialize(
                receiver.position.x,
                Random.Range(minSpeed, maxSpeed));
        }
    }
}