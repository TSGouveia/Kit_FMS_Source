using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool unique;

    [HideInInspector]
    public Transform parentAfterDrag;

    [HideInInspector]
    public int childIndex;

    [SerializeField]
    private Transform firstParent;

    [HideInInspector]
    public Transform lastParent;

    private bool spawnOnEndDrag = false;


    private CanvasGroup canvasGroup;

    private ChooseColor chooseColor;

    private PunchButtons3D punchButtons;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        chooseColor = FindObjectOfType<ChooseColor>();
        punchButtons = FindObjectOfType<PunchButtons3D>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (transform.parent == firstParent)
            {
                spawnOnEndDrag = true;
            }
            lastParent = transform.parent;

            childIndex = transform.GetSiblingIndex();
            parentAfterDrag = transform.parent;

            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (parentAfterDrag != firstParent && spawnOnEndDrag)
            {
                spawnOnEndDrag = false;
                if (!unique)
                {
                    GameObject newGO = Instantiate(gameObject, firstParent);

                    newGO.transform.SetSiblingIndex(childIndex);
                    CanvasGroup canvasGroupNewGO = newGO.GetComponent<CanvasGroup>();
                    canvasGroupNewGO.blocksRaycasts = true;
                }
                else
                {
                    chooseColor.StartChoosingColor(transform.GetChild(0).GetComponent<TMP_Text>().text);
                }
            }

            transform.SetParent(parentAfterDrag);
            transform.SetSiblingIndex(childIndex);
            canvasGroup.blocksRaycasts = true;
        }
    }
}
