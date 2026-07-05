using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingStationController : MonoBehaviour
{
    public static readonly List<SortingStationController> Stations = new List<SortingStationController>();

    [Header("Station")]
    public string targetClass = "Missing USB";
    public Transform sortingPoint;
    public float stopDistance = 0.08f;
    public bool debugLog = true;

    [Header("Cylinder")]
    public bool useCylinder = true;
    public Transform rod;
    public Vector3 rodExtendOffset = new Vector3(0f, 0f, -0.35f);
    public float rodSpeed = 0.8f;
    public float holdTime = 0.2f;

    [Header("PCB Path")]
    public Transform pushTarget;
    public Transform entryPoint;
    public float pcbPushSpeed = 0.8f;
    public float pcbEntrySpeed = 0.8f;

    [Header("Tray")]
    public TrayController tray;

    private bool busy = false;
    private Vector3 rodHome;
    private Vector3 rodExtend;

    void OnEnable()
    {
        if (!Stations.Contains(this))
            Stations.Add(this);
    }

    void OnDisable()
    {
        Stations.Remove(this);
    }

    void Start()
    {
        CacheRodPositions();
    }

    void CacheRodPositions()
    {
        if (rod == null)
            return;

        rodHome = rod.localPosition;
        rodExtend = rodHome + rodExtendOffset;
    }

    public bool TryStartSorting(PCBMove pcb)
    {
        if (busy || pcb == null || sortingPoint == null)
            return false;

        PCBInfo info = pcb.GetComponent<PCBInfo>();

        if (info == null)
            return false;

        if (!info.inspected)
            return false;

        if (pcb.sorted)
            return false;

        if (!ClassMatches(info.defectClass, targetClass))
            return false;

        float distance = Mathf.Abs(pcb.transform.position.x - sortingPoint.position.x);

        if (distance > stopDistance)
            return false;

        StartCoroutine(ProcessSorting(pcb, info));
        return true;
    }

    bool ClassMatches(string detectedClass, string target)
    {
        return PCBInfo.ClassMatches(detectedClass, target);
    }

    IEnumerator ProcessSorting(PCBMove pcb, PCBInfo info)
    {
        busy = true;

        pcb.StopMove();
        pcb.MarkSorted();

        if (debugLog)
            Debug.Log("Sorting Start: " + PCBInfo.ToDisplayClassName(targetClass) + " | PCBInfo = " + PCBInfo.ToDisplayClassName(info.defectClass));

        if (useCylinder && rod != null)
        {
            CacheRodPositions();
            yield return ExtendRodAndPushPCB(pcb);
            yield return new WaitForSeconds(holdTime);
            yield return RetractRod();
        }
        else
        {
            if (pushTarget != null)
                yield return pcb.MoveToTarget(pushTarget.position, pcbPushSpeed);
        }

        if (entryPoint != null)
            yield return pcb.MoveToTarget(entryPoint.position, pcbEntrySpeed);

        if (tray != null)
            yield return tray.StorePCB(pcb);

        if (debugLog)
            Debug.Log("Sorting Finish: " + PCBInfo.ToDisplayClassName(targetClass));

        busy = false;
    }

    IEnumerator ExtendRodAndPushPCB(PCBMove pcb)
    {
        while (Vector3.Distance(rod.localPosition, rodExtend) > 0.001f)
        {
            rod.localPosition = Vector3.MoveTowards(
                rod.localPosition,
                rodExtend,
                rodSpeed * Time.deltaTime
            );

            if (pushTarget != null)
            {
                pcb.MoveStepToTarget(pushTarget.position, pcbPushSpeed);
            }

            yield return null;
        }

        rod.localPosition = rodExtend;

        if (pushTarget != null)
        {
            yield return pcb.MoveToTarget(pushTarget.position, pcbPushSpeed);
        }
    }

    IEnumerator RetractRod()
    {
        while (Vector3.Distance(rod.localPosition, rodHome) > 0.001f)
        {
            rod.localPosition = Vector3.MoveTowards(
                rod.localPosition,
                rodHome,
                rodSpeed * Time.deltaTime
            );

            yield return null;
        }

        rod.localPosition = rodHome;
    }

    [ContextMenu("Test Cylinder")]
    public void TestCylinder()
    {
        if (Application.isPlaying)
            StartCoroutine(TestCylinderRoutine());
        else
            Debug.LogWarning("Test Cylinder chỉ chạy khi đang Play.");
    }

    IEnumerator TestCylinderRoutine()
    {
        if (rod == null)
        {
            Debug.LogWarning("Chưa gán Rod.");
            yield break;
        }

        CacheRodPositions();
        yield return RetractRod();
        yield return ExtendRodOnly();
        yield return new WaitForSeconds(holdTime);
        yield return RetractRod();
    }

    IEnumerator ExtendRodOnly()
    {
        while (Vector3.Distance(rod.localPosition, rodExtend) > 0.001f)
        {
            rod.localPosition = Vector3.MoveTowards(
                rod.localPosition,
                rodExtend,
                rodSpeed * Time.deltaTime
            );

            yield return null;
        }

        rod.localPosition = rodExtend;
    }
}
