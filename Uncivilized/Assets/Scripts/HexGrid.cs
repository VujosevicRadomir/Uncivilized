using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexGrid : MonoBehaviour { 

	// Use this for initialization
	void Start () {
       
        NumCols = 60;
        NumRows = 30;
        HexToGameObjectMap = new Dictionary<Hex, GameObject>();
        hexes = new Hex[NumCols + 1, NumRows + 1];
        TileGraph = new int[NumCols, NumRows];
        GenerateMap();
        
	}

    public static int [,] TileGraph;
    public static Hex[,] hexes;
    public static Dictionary<Hex, GameObject> HexToGameObjectMap;

    public static List<Unit> units;
    public static Dictionary<Unit, GameObject> UnitToGameObjectMap;

    public GameObject HexPrefab;
    public GameObject ForestPrefab;

    public GameObject JunglePrefab;

    public GameObject UnitDwarfPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlatland;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public float HeightMountain = 0.85f;
    public float HeightHill = 0.6f;
    public float HeightFlat = 0.0f;

    public float MoistureJungle = 1f;
    public float MoistureForest = 0.8f;
    public float MoistureGrasslands = 0.33f;
    public float MoisturePlains = 0f;


    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatMountain;
    public Material MatDesert;
    public Material MatForest;

    public static int NumRows;
    public static int NumCols;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (units != null)
            {
                foreach (Unit u in units)
                {
                    u.DoTurn();
                }
            }
        }
    }
    

    public static Hex GetHexAt(int col, int row)
    {
        if(hexes == null)
        {
            Debug.Log("Hexes array not instantiated");
            return null;
        }

        while(col < 0)
        {
            col += NumCols;
        }
        col = col % NumCols;

        if (row < 0 || row >= NumRows)
        {
            Debug.Log("Invalid row requested");
        }

        return hexes[col , row];
    }

    virtual public void GenerateMap()
    {
        for (int column = 0;  column < NumCols;  column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex h = new Hex(column, row);

                hexes[column, row] = h;

                h.Elevation = -0.5f;
                h.Moisture = 0f;

                Vector3 pos = h.PositionFromCamera(
                    Camera.main.transform.position,
                    NumCols,
                    NumRows
                    );
                    
                GameObject go = Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    this.transform
                    );

                HexToGameObjectMap[h] = go;
                go.GetComponent<HexComponent>().Hex = h;
                go.GetComponent<HexComponent>().HexGrid = this;

                go.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);
               
                
            }
        }


        Unit unit = new Unit();
        SpawnUnitAt(unit, UnitDwarfPrefab, 58, 4);

        UpdateHexVisuals();
    }

    public void UpdateHexVisuals()
    {
        for (int column = 0; column < NumCols; column++ )
        {
            for (int row = 0; row < NumRows; row++)
            {

                Hex h = hexes[column, row];
                GameObject hexGO = HexToGameObjectMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                if(h.Elevation >= HeightMountain)
                {
                    h.TerrainType = Hex.TerrainTypeEnum.Mountain;
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                }
                else if (h.Elevation >= HeightHill)
                {
                    h.TerrainType = Hex.TerrainTypeEnum.PlainsHill;
                    mf.mesh = MeshHill;

                }
                else if (h.Elevation >= HeightFlat)
                {

                    h.TerrainType = Hex.TerrainTypeEnum.Plains;
                    mf.mesh = MeshFlatland;
                }
                else
                {

                    h.TerrainType = Hex.TerrainTypeEnum.Water;
                    mr.material = MatOcean; 
                    mf.mesh = MeshWater;
                }
                if ((h.Elevation >= HeightFlat) && (h.Elevation < HeightMountain))
                {
                    
                    if (h.Moisture >= MoistureJungle)
                    {

                        h.TerrainFeature = Hex.TerrainFeatureEnum.Jungle;
                        mr.material = MatGrassland;
                        GameObject go = Instantiate(JunglePrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        if (h.Elevation >= HeightHill)
                        {
                            go.transform.Translate(new Vector3(0, 0.25f, 0));
                        }
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        h.TerrainFeature = Hex.TerrainFeatureEnum.Forrest;
                        mr.material = MatGrassland;
                        
                        GameObject go = Instantiate(ForestPrefab, hexGO.transform.position, Quaternion.identity, hexGO.transform);
                        if (h.Elevation >= HeightHill)
                        {
                            go.transform.Translate(new Vector3(0, 0.25f, 0));
                        } 

                    }
                    else if (h.Moisture >= MoistureGrasslands)
                    {
                        h.TerrainFeature = Hex.TerrainFeatureEnum.None;
                        if(h.TerrainType == Hex.TerrainTypeEnum.PlainsHill)
                        {
                            h.TerrainType = Hex.TerrainTypeEnum.GrasllandHill;
                        }
                        else
                        {
                            h.TerrainType = Hex.TerrainTypeEnum.Grassland;
                        }
                        mr.material = MatGrassland;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        h.TerrainFeature = Hex.TerrainFeatureEnum.None;
                        mr.material = MatPlains;
                    }
                    else
                    {
                        h.TerrainType = Hex.TerrainTypeEnum.Desert;
                        h.TerrainFeature = Hex.TerrainFeatureEnum.None;
                        mr.material = MatDesert;
                    }
                }

            }
        }
    }

    public Hex [] GetHexesInRadius(Hex centerHex, int radius)
    {
        List<Hex> results = new List<Hex>();

        
        for (int dx = -radius + 1; dx < radius; dx++)
        {
            
            for (int dy = Mathf.Max(-radius, -dx - radius) + 1 ; dy < Mathf.Min(radius, -dx + radius); dy++)
            {
                
                results.Add(GetHexAt(centerHex.Q + dx, centerHex.R + dy));
                
            }
        }

        return results.ToArray();
    }

    public static int DistanceBetweenHexes(Hex a, Hex b)
    {
        int dQ = Mathf.Abs(a.Q - b.Q);
        if (dQ > NumCols / 2)
            dQ = NumCols - 2;

        return Mathf.Max(
            dQ,
            Mathf.Abs(a.R - b.R),
            Mathf.Abs(a.S - b.S)
            );
            
    }

 

    public static List<Hex> ShortestPathBetweenHexes(Hex start, Hex goal)
    {
        List<Hex> result = new List<Hex>();

        Dictionary<Hex, int> costToReach = new Dictionary<Hex, int>();
        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
        Dictionary<Hex, int> frontier = new Dictionary<Hex, int>();

        costToReach.Add(start, 0);
        frontier.Add(start, 0);


        while(frontier.Count > 0)
        {
            Hex current = null;
            int min = 0;
            bool minSet = false;
            foreach(var entry in frontier)
            {
                if(minSet == false | min > entry.Value)
                {
                    current = entry.Key;
                    min = entry.Value;
                    minSet = true;
                }
            }
            frontier.Remove(current);

            if (current.Equals(goal))
            {
                break;
            }

            List<Hex> neighbours = HexGrid.GetHexNeighbours(current);
            
            if(neighbours == null)
            {
                continue;
            }
            
            
            foreach (Hex next in neighbours)
            {

                int newCost = costToReach[current] + next.CostToEnterTile();

                if ((!costToReach.ContainsKey(next)) || (costToReach[next] > newCost))
                {
                    costToReach[next] = newCost;
                    frontier[next] = DistanceBetweenHexes(next, goal) * next.CostToEnterTile();
                    cameFrom[next] = current;
                }
            }
        }

        if (!costToReach.ContainsKey(goal))
        {
            Debug.Log("Can't reach goal");
            return result;
        }

        result.Add(goal);
        while(start != goal)
        {
            
            result.Add(cameFrom[goal]);
            goal = cameFrom[goal];
        }

        result.Reverse();
        return result;
    }

    

    public void SpawnUnitAt(Unit unit, GameObject unitPrefab, int col, int row)
    {
        if(units == null)
        {
            units = new List<Unit>();
            UnitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }
        Hex h = GetHexAt(col, row);
        GameObject myHex = HexToGameObjectMap[h];

        units.Add(unit);
        

        GameObject unitGO = Instantiate(unitPrefab, myHex.transform.position, Quaternion.identity);
        UnitToGameObjectMap[unit] = unitGO;
        unit.SetHex(h);
        
    }

    public static List<Hex> GetHexNeighbours(Hex hex)
    {
        List<Hex> result = new List<Hex>();

        result.Add(GetHexAt(hex.Q + 1, hex.R));
        result.Add(GetHexAt(hex.Q - 1, hex.R));

        if(hex.R > 0)
        {
            result.Add(GetHexAt(hex.Q, hex.R - 1));
            result.Add(GetHexAt(hex.Q + 1, hex.R - 1));
        }

        if(hex.R + 1 < NumRows)
        {
            result.Add(GetHexAt(hex.Q, hex.R + 1));
            result.Add(GetHexAt(hex.Q - 1, hex.R + 1));
        }


        return result.Where(r => r.TerrainType != Hex.TerrainTypeEnum.Mountain).ToList();
    }
}
