using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    public string Name = "Unnamed";
    public int hitpoints = 10;
    public int strength = 8;
    public int movement = 2;
    public int movementRemaining = 2;
    public Vector3 velocity;

    public Hex Hex { get; protected set; }

    Queue<Hex> HexPath;

    public void SetHex(Hex hex)
    {
        if (Hex != null)
        {
            Hex.RemoveUnit(this);
        }
        Hex oldHex = Hex;
        Hex = hex;
        hex.AddUnit(this);

        GameObject unitGO = HexGrid.UnitToGameObjectMap[this];
        GameObject hexGO = HexGrid.HexToGameObjectMap[hex];
        unitGO.transform.parent = hexGO.transform;

        velocity = Vector3.zero;
        unitGO.transform.localPosition = Vector3.zero;
    }


    public void MoveUnit(Hex targetHex)
    {
        SetHex(targetHex);
    }

    public void MoveUnit(int moveX, int moveY)
    {
        Hex targetHex = HexGrid.GetHexAt(Hex.Q + moveX, Hex.R + moveY);
        SetHex(targetHex);
    }

    public void DoTurn()
    {
        if (HexPath == null)
        {
            return;
        }
        Hex newHex = HexPath.Dequeue();
        SetHex(newHex);


    }

    public void SetHexPath(Hex [] hexPath)
    {
        HexPath = new Queue<Hex>(hexPath);
        
        
    }
    
   
}
