using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hex {



    public Hex(int q, int r)
    {
        Q = q;
        R = r;
        S = -Q - R;

    }

    public readonly int Q; //column
    public readonly int R; //row
    public readonly int S; // S = -Q - R

    public float Moisture;
    public float Elevation;
    public enum TerrainTypeEnum { Grassland, Plains, Water, Mountain, PlainsHill, GrasllandHill, Desert}
    public enum TerrainFeatureEnum { None, Forrest, Jungle}
    public TerrainTypeEnum TerrainType;
    public TerrainFeatureEnum TerrainFeature;

    public HashSet<Unit> units;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    float radius = 1f;

    public Vector3 Position()
    {
        return new Vector3(
            HexHorizontalSpacing() * (Q + R / 2f),
            0,
            HexVerticalSpacing() * R
            );
    }

    public float HexHeight()
    {
        return 2 * radius;
    }

    public float HexWidth()
    {
        return HexHeight() * WIDTH_MULTIPLIER;
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }
    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numCols, float numRows)
    {
        // mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numCols * HexHorizontalSpacing();

        Vector3 position = Position();

        float widthsFromCamera = (position.x - cameraPosition.x) / mapWidth;


        if (widthsFromCamera > 0)
        {
            widthsFromCamera += 0.5f;
        }
        else
        {
            widthsFromCamera -= 0.5f;
        }

        int widthsToFix = (int)widthsFromCamera;

        position.x -= widthsToFix * mapWidth;

        return position;
    }

    public void AddUnit(Unit unit)
    {
        if (units == null)
        {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
        
    }

    public void RemoveUnit(Unit unit)
    {
        if (units != null)
        {
            units.Remove(unit);
        }
    }

    public Unit[] Units()
    {
        return units.ToArray();
    }

    public int CostToEnterTile()
    {
        switch(TerrainType)
        {
            case TerrainTypeEnum.PlainsHill:
            case TerrainTypeEnum.GrasllandHill:
                return 2;
            case TerrainTypeEnum.Mountain:
                return 1000;
            case TerrainTypeEnum.Water:
                return 10;
            default:
                return 1;
        }
    }

    public override string ToString()
    {
         return "Hex : " + Q + " , " + R;
    }
}
