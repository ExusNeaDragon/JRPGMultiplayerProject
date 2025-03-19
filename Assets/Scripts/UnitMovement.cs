using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Vector2 gridStartPos;
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float tileSize = 1f;
    public float moveSpeed = 0.2f;

    private bool isMoving = false;
    private Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();
        gridStartPos = transform.position - new Vector3((gridWidth - 1) * 0.5f * tileSize, (gridHeight - 1) * 0.5f * tileSize, 0);
        gridPosition = new Vector2Int(gridWidth / 2, gridHeight / 2);
        UpdateWorldPosition(true);
    }

    public void Move(Vector2Int direction)
    {
        if (isMoving) return;

        gridPosition += direction;
        UpdateWorldPosition();

        if (unit.isPlayer)
        {
            unit.battleManager.EndTurn();
        }
    }

    void UpdateWorldPosition(bool instant = false)
    {
        Vector2 newPosition = gridStartPos + new Vector2(gridPosition.x * tileSize, gridPosition.y * tileSize);

        if (instant)
        {
            transform.position = newPosition; // Instantly set position on spawn
        }
        else
        {
            StartCoroutine(SmoothMove(newPosition));
        }
    }

    IEnumerator SmoothMove(Vector2 targetPos)
    {
        isMoving = true;
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
