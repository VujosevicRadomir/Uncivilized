using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour {

    public Hex Hex;
    public HexGrid HexGrid;

    private Color StartingColor;
    private void Start()
    {
        StartingColor = GetComponentInChildren<MeshRenderer>().material.color;
    }
    public void UpdatePosition()
    {
        
        this.transform.position = Hex.PositionFromCamera(
            Camera.main.transform.position,
            HexGrid.NumCols,
            HexGrid.NumRows
            );
    }

    private void OnMouseDown()
    {
       if(StateManager.HexSelected == false)
        {
            StateManager.HexSelected = true;
            StateManager.SelectedHex = Hex;
        }
        else
        {
            StateManager.HexSelected = false;
            StateManager.SelectedHex = null;
        }
    }

    public void HighlightHex()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
    }

    public void UnHighlightHex()
    {
        GetComponentInChildren<MeshRenderer>().material.color = StartingColor;
    }

    List<Hex> HighlightedHexes;
    private void OnMouseEnter()
    {
        if(Hex != StateManager.SelectedHex & StateManager.HexSelected == true)
        {
            if(HighlightedHexes == null)
            {
                HighlightedHexes = new List<Hex>();
            }
            foreach(Hex h in HexGrid.ShortestPathBetweenHexes(StateManager.SelectedHex, Hex))
            {
                HighlightedHexes.Add(h);
                HexGrid.HexToGameObjectMap[h].GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            }
        }

        if (Hex.TerrainType != Hex.TerrainTypeEnum.Mountain & Hex.TerrainType != Hex.TerrainTypeEnum.Water)
        {
               // HighlightHex();
        }
    }
    private void OnMouseExit()
    {
        if (HighlightedHexes == null) return;
        foreach(Hex h in HighlightedHexes)
        {
            GameObject HCGO = HexGrid.HexToGameObjectMap[h];
            HCGO.GetComponentInChildren<MeshRenderer>().material.color = HCGO.GetComponent<HexComponent>().StartingColor;
        }
        if (Hex.TerrainType != Hex.TerrainTypeEnum.Mountain & Hex.TerrainType != Hex.TerrainTypeEnum.Water)
        {
           // UnHighlightHex();
        }
    }
}
