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

    public Queue<Hex> MoveQueue;
    
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


    public void SetMoveQueue(Hex targetHex)
    {
        MoveQueue = new Queue<Hex>(HexGrid.ShortestPathBetweenHexes(Hex, targetHex));

        MoveQueue.Dequeue();
        
    }

    public void MoveUnit(int moveX, int moveY)
    {
        Hex targetHex = HexGrid.GetHexAt(Hex.Q + moveX, Hex.R + moveY);
        SetHex(targetHex);
    }

    public void DoTurn()
    {
        
        HandleMovement();
        ResetStats();
    }

    public void HandleMovement()
    {
        if (MoveQueue == null || MoveQueue.Count == 0)
        {
            return;
        }

        Hex current = MoveQueue.Peek();
        if(movementRemaining >= current.CostToEnterTile() || movementRemaining == movement)
        {
            MoveToNextHex();
            HandleMovement();
        }
        else
        {
            return;
        }
            

    }

    public void MoveToNextHex()
    {
       
        Hex h = MoveQueue.Dequeue();
        SetHex(h);
        movementRemaining -= h.CostToEnterTile();
    }

    public void ResetStats()
    {
        movementRemaining = movement;
    }
   

}
