using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("小车车")]
    public BackgroundCar[] carPrefabs;

    [Header("接收器")]
    public Transform receiver;

    [Header("频率")]
    public float minInterval = 5f;
    public float maxInterval = 12f;

    [Header("速度")]
    public float minSpeed = 3f;
    public float maxSpeed = 5f;

    private Coroutine spawnRoutine;

    [Header("Z轴偏差")]
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

            //各个车高度不同
            spawnPosition.y += prefab.spawnYOffset;

            //随机Z
            spawnPosition.z += Random.Range(minZOffset, maxZOffset);

            BackgroundCar car = Instantiate(
                prefab, spawnPosition, prefab.transform.rotation);

            car.Initialize(
                receiver.position.x,
                Random.Range(minSpeed, maxSpeed));
        }
    }
}