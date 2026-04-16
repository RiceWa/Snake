using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private float minX = -16f;
    [SerializeField] private float maxX = 16f;
    [SerializeField] private float minY = -8f;
    [SerializeField] private float maxY = 8f;

    private GameObject currentFood;

    private void Start()
    {
        SpawnFood();
    }

    private void Update()
    {
        if (currentFood == null)
        {
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(
            Mathf.Round(Random.Range(minX, maxX)),
            Mathf.Round(Random.Range(minY, maxY)),
            0f
        );

        currentFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }
}