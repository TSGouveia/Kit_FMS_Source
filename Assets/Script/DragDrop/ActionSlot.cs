using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ActionSlot : MonoBehaviour, IDropHandler
{
    private Transform actions;
    private Slots slotsScript;

    public static bool leftIsOnDraggables = true;

    private SetupLaunchManager setupLaunchManager;
    private void Start()
    {
        actions = transform.parent;
        slotsScript = FindObjectOfType<Slots>();
        setupLaunchManager = FindObjectOfType<SetupLaunchManager>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (transform.childCount == 0)
            {
                GameObject dropped = eventData.pointerDrag;
                DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
                draggableItem.parentAfterDrag = transform;
                if (transform != draggableItem.lastParent && draggableItem.lastParent.parent != transform.parent)
                {
                    if (dropped.transform.GetChild(0).GetComponent<TMP_Text>().text == "Color 1")
                    {
                        leftIsOnDraggables = false;
                    }

                    slotsScript.AddSlot();
                }
            }
            else
            {
                GameObject dropped = eventData.pointerDrag;
                DraggableItem newDraggableItem = dropped.GetComponent<DraggableItem>();
                Transform newParent = newDraggableItem.lastParent;

                Transform oldGO = transform.GetChild(0);

                if (transform != newDraggableItem.lastParent && newDraggableItem.lastParent.parent != transform.parent)
                {
                    if (dropped.transform.GetChild(0).GetComponent<TMP_Text>().text == "Color 1")
                    {
                        leftIsOnDraggables = false;
                    }
                }

                if (newParent.parent != actions)
                {
                    //It came from the draggables
                    RemoveFromSlot(oldGO, newParent);
                }
                else
                {
                    //It came from slot
                    oldGO.SetParent(newParent);
                }

                newDraggableItem.parentAfterDrag = transform;
            }
        }
    }
    IEnumerator ReturnColorToDraggable(Transform oldGO, Transform newParent,int childIndex)
    {
        yield return new WaitForEndOfFrame();
        oldGO.SetParent(newParent);
        oldGO.SetSiblingIndex(childIndex);
    }

    private void RemoveFromSlot(Transform oldGO,Transform newParent)
    {
        string buttonText = oldGO.GetChild(0).GetComponent<TMP_Text>().text;
        int childIndex = -1;
        if (buttonText == "Color 1")
        {
            childIndex = 0;
            leftIsOnDraggables = true;

            setupLaunchManager.DisablePieceLeft();
        }
        else if (buttonText == "Color 2")
        {
            if (leftIsOnDraggables)
                childIndex = 1;
            else
                childIndex = 0;

            setupLaunchManager.DisablePieceRight();
        }
        if (childIndex == -1)
        {
            Destroy(oldGO.gameObject);
        }
        else
        {
            StartCoroutine(ReturnColorToDraggable(oldGO, newParent, childIndex));
        }
    }
}

