using System.Collections;
using UnityEngine;

public class PCBMove : MonoBehaviour
{
    [Header("Move")]
    public float speed = 0.8f;

    [Header("State")]
    public bool inspected = false;
    public bool sorted = false;

    private bool canMove = true;
    private bool inspectionRequested = false;
    private bool sortingRequested = false;
    private bool isMovingToTarget = false;

    void Update()
    {
        if (canMove && !isMovingToTarget)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
        }

        CheckInspectionPoint();
        CheckSortingStations();
    }

    void CheckInspectionPoint()
    {
        if (inspected || inspectionRequested)
            return;

        if (VisionManager.Instance == null || VisionManager.Instance.inspectionPoint == null)
            return;

        float distance = Mathf.Abs(
            transform.position.x - VisionManager.Instance.inspectionPoint.position.x
        );

        if (distance <= VisionManager.Instance.stopDistance)
        {
            bool accepted = VisionManager.Instance.StartInspection(this);

            if (accepted)
                inspectionRequested = true;
        }
    }

    void CheckSortingStations()
    {
        if (!inspected || sorted || sortingRequested || isMovingToTarget)
            return;

        if (SortingStationController.Stations.Count == 0)
            return;

        foreach (SortingStationController station in SortingStationController.Stations)
        {
            if (station == null)
                continue;

            bool accepted = station.TryStartSorting(this);

            if (accepted)
            {
                sortingRequested = true;
                break;
            }
        }
    }

    public void StopMove()
    {
        canMove = false;
    }

    public void ResumeMove()
    {
        if (!sorted && !isMovingToTarget)
            canMove = true;
    }

    public void MarkInspected()
    {
        inspected = true;
    }

    public void MarkSorted()
    {
        sorted = true;
        canMove = false;
    }

    public void MoveStepToTarget(Vector3 targetPosition, float moveSpeed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    public IEnumerator MoveToTarget(Vector3 targetPosition, float moveSpeed)
    {
        isMovingToTarget = true;
        canMove = false;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            MoveStepToTarget(targetPosition, moveSpeed);
            yield return null;
        }

        transform.position = targetPosition;
        isMovingToTarget = false;
    }
}
