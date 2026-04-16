using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviour
{
    // Movement settings
    [SerializeField] private float moveRate = 0.2f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;

    // Body segment prefab and list of current body parts
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private List<Transform> bodySegments = new List<Transform>();

    // Food tracking for speed increase
    [SerializeField] private int foodEaten = 0;
    [SerializeField] private int foodsPerSpeedIncrease = 5;
    [SerializeField] private float minimumMoveRate = 0.05f;

    private float moveTimer;
    private bool isGameOver = false;

    private void Update()
    {
        if (isGameOver) return;

        HandleInput();

        moveTimer += Time.deltaTime;

        if (moveTimer >= moveRate)
        {
            MoveSnake();
            moveTimer = 0f;
        }
    }

    private void HandleInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Prevent reversing directly into yourself
        if (keyboard.wKey.wasPressedThisFrame && moveDirection != Vector2.down)
            moveDirection = Vector2.up;
        else if (keyboard.sKey.wasPressedThisFrame && moveDirection != Vector2.up)
            moveDirection = Vector2.down;
        else if (keyboard.aKey.wasPressedThisFrame && moveDirection != Vector2.right)
            moveDirection = Vector2.left;
        else if (keyboard.dKey.wasPressedThisFrame && moveDirection != Vector2.left)
            moveDirection = Vector2.right;
    }

    private void MoveSnake()
    {
        // Store head's previous position
        Vector3 previousPosition = transform.position;

        // Move the head one step
        transform.position += (Vector3)moveDirection;

        // Move each body segment to the previous position of the segment in front
        for (int i = 0; i < bodySegments.Count; i++)
        {
            Vector3 currentSegmentPosition = bodySegments[i].position;
            bodySegments[i].position = previousPosition;
            previousPosition = currentSegmentPosition;
        }
    }

    private void Grow()
    {
        Vector3 spawnPosition;

        // Spawn first segment behind the head
        if (bodySegments.Count == 0)
        {
            spawnPosition = transform.position - (Vector3)moveDirection;
        }
        else
        {
            // Spawn new segment at the last segment's position
            spawnPosition = bodySegments[bodySegments.Count - 1].position;
        }

        GameObject newSegment = Instantiate(bodyPrefab, spawnPosition, Quaternion.identity);
        bodySegments.Add(newSegment.transform);
    }

    private void IncreaseSpeed()
    {
        if (foodEaten % foodsPerSpeedIncrease == 0)
        {
            // Double the snake speed by halving the delay
            moveRate *= 0.5f;

            // Prevent it from becoming too fast
            moveRate = Mathf.Max(moveRate, minimumMoveRate);
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Eat food, grow, and track speed increase
        if (other.CompareTag("Food"))
        {
            Grow();
            Destroy(other.gameObject);

            foodEaten++;
            IncreaseSpeed();
        }

        // Die on body or wall collision
        if (other.CompareTag("Body") || other.CompareTag("Wall"))
        {
            GameOver();
        }
    }
}