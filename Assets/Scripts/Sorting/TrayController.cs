using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayController : MonoBehaviour
{
    [Header("Slot Settings")]
    public List<Transform> slots = new List<Transform>();
    public int capacity = 10;

    [Header("Motion")]
    public float placeSpeed = 1.0f;
    public bool parentPCBToTray = true;
    public bool destroyOldestWhenFull = true;

    private readonly List<PCBMove> storedPCBs = new List<PCBMove>();

    void Awake()
    {
        AutoLoadSlotsIfEmpty();
    }

    void AutoLoadSlotsIfEmpty()
    {
        if (slots.Count > 0)
            return;

        Transform slotsRoot = transform.Find("Slots");

        if (slotsRoot == null)
            return;

        for (int i = 0; i < slotsRoot.childCount; i++)
        {
            slots.Add(slotsRoot.GetChild(i));
        }
    }

    public IEnumerator StorePCB(PCBMove pcb)
    {
        if (pcb == null)
            yield break;

        if (slots.Count == 0)
        {
            Debug.LogWarning("TrayController chưa có Slot: " + gameObject.name);
            yield break;
        }

        int maxCount = Mathf.Min(capacity, slots.Count);

        if (storedPCBs.Count >= maxCount)
        {
            if (destroyOldestWhenFull && storedPCBs[0] != null)
            {
                Destroy(storedPCBs[0].gameObject);
            }

            storedPCBs.RemoveAt(0);
            RepositionExistingPCBs();
        }

        int slotIndex = Mathf.Clamp(storedPCBs.Count, 0, maxCount - 1);
        Transform targetSlot = slots[slotIndex];

        yield return pcb.MoveToTarget(targetSlot.position, placeSpeed);

        pcb.transform.position = targetSlot.position;
        pcb.transform.rotation = targetSlot.rotation;

        if (parentPCBToTray)
            pcb.transform.SetParent(transform);

        storedPCBs.Add(pcb);

        Debug.Log("PCB stored in tray: " + gameObject.name + " | Count = " + storedPCBs.Count);
    }

    void RepositionExistingPCBs()
    {
        for (int i = 0; i < storedPCBs.Count; i++)
        {
            if (storedPCBs[i] == null || i >= slots.Count)
                continue;

            storedPCBs[i].transform.position = slots[i].position;
            storedPCBs[i].transform.rotation = slots[i].rotation;
        }
    }
}
