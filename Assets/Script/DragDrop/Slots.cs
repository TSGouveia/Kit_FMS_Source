using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{
    [SerializeField]
    GameObject slotPrefab;

    [SerializeField]
    Transform scrollParent;

    private void Start()
    {
        AddSlot();
    }
    public void AddSlot()
    {
        Instantiate(slotPrefab, scrollParent);
    }
    public void RemoveSlot()
    {
        Debug.Log(scrollParent.childCount);

        if (scrollParent.childCount <= 0)
            return;

        Destroy(scrollParent.GetChild(scrollParent.childCount - 1).gameObject);
    }
}
