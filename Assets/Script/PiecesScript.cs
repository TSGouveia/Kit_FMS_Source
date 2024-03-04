using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesScript : MonoBehaviour
{
    [SerializeField]
    private Material[] colorMaterials;
    [SerializeField]
    private GameObject piece1;
    [SerializeField]
    private GameObject piece2;

    public void InsertPiece(int pos, int color) // RGBY 0123
    {
        Debug.Log("Changin piece color");
        switch (pos)
        {
            case 1:
                if (piece1.activeSelf)
                {
                    Debug.LogWarning("Piece 1 is already active");
                }
                else
                {
                    piece1.GetComponent<Renderer>().material = colorMaterials[color];
                    piece1.SetActive(true);
                }
                break;
            case 2:
                if (piece2.activeSelf)
                {
                    Debug.LogWarning("Piece 2 is already active");
                }
                else
                {
                    piece2.GetComponent<Renderer>().material = colorMaterials[color];
                    piece2.SetActive(true);
                }
                break;
        }
    }
}