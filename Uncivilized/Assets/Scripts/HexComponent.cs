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
            StateManager.SelectedHex = Hex;
        }
        else
        {
            Unit u = StateManager.SelectedUnit;
            u.SetMoveQueue(Hex);
            
            StateManager.SelectedHex = null;

        }
    }

    public void HighlightHex()
    {
        GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
    }

    public void UnHighlightHex()
    {
        GetComponentInChildren<MeshRenderer>().material.color = StartingColor;
    }

    List<Hex> HighlightedHexes;
    private void OnMouseEnter()
    {
        MovementPath();
    }

    public void MovementPath()
    {
        if (StateManager.HexSelected == true && (Hex != StateManager.SelectedHex ))
        {
            if (Hex.TerrainType == Hex.TerrainTypeEnum.Mountain)
            {
                Debug.Log("Can't move into a mountain");
                return;
            }
            if (HighlightedHexes == null)
            {
                HighlightedHexes = new List<Hex>();
            }


            List<Hex> hexes = HexGrid.ShortestPathBetweenHexes(StateManager.SelectedHex, Hex);
            if (hexes.Count > 0)
            {
                foreach (Hex h in hexes)
                {
                    HighlightedHexes.Add(h);
                    HexGrid.HexToGameObjectMap[h].GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
                }
            }
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
       
    }
}
