using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractWithDraggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const string leftText = "Color 1";
    private const string rightText = "Color 2";

    private ChooseColor chooseColor;

    [SerializeField]
    private bool left;

    [SerializeField]
    private bool color;

    private bool pointerIsOver = false;

    private Transform draggableParent;
    
    private SetupLaunchManager setupLaunchManager;
    private void Start()
    {
        chooseColor = FindObjectOfType<ChooseColor>();
        draggableParent = transform.parent;
        setupLaunchManager = FindObjectOfType<SetupLaunchManager>();
    }

    private void Update()
    {
        if (pointerIsOver)
        {
            if (Input.GetMouseButtonDown(1) && color)
            {
                string side;

                if (left)
                    side = leftText;
                else
                    side = rightText;

                chooseColor.StartChoosingColor(side);
            }
            if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Delete))
            {
                string buttonText = transform.GetChild(0).GetComponent<TMP_Text>().text;
                int childIndex = -1;
                if (buttonText == "Color 1")
                {
                    childIndex = 0;
                    ActionSlot.leftIsOnDraggables = true;

                    setupLaunchManager.DisablePieceLeft();
                }
                else if (buttonText == "Color 2")
                {
                    if (ActionSlot.leftIsOnDraggables)
                        childIndex = 1;
                    else
                        childIndex = 0;

                    setupLaunchManager.DisablePieceRight();
                }
                if (childIndex == -1)
                {
                    Destroy(transform.parent.gameObject);
                    Destroy(transform.gameObject);
                }
                else
                {
                    StartCoroutine(ReturnColorToDraggable(transform, draggableParent, childIndex));
                }
            }
        }
    }

    IEnumerator ReturnColorToDraggable(Transform oldGO, Transform newParent, int childIndex)
    {
        yield return new WaitForEndOfFrame();

        Transform oldParent = oldGO.parent;
        oldGO.SetParent(newParent);
        oldGO.SetSiblingIndex(childIndex);

        Destroy(oldParent.gameObject);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsOver = false;
    }
}
