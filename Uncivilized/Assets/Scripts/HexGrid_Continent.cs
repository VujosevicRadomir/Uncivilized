using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid_Continent :HexGrid {
    
    override public void GenerateMap()
    {
        base.GenerateMap();

        int numContinents = 3;
        int continentSpacing = NumCols / numContinents;

        //Random.InitState(0);

        for (int c = 0; c < numContinents; c++)
        {
            int numSplats = Random.Range(3, 7);
            for (int i = 0; i < numSplats; i++)
            {
                int radius = Random.Range(5, 8);
                
                int y = Random.Range(radius, NumRows - radius);
                int x = Random.Range(0, 10) - y / 2 + c * continentSpacing;

                ElevateArea(x, y, radius);
            }

        }
        float noiseResolution = 0.01f;
        float noiseScale = 2f;

        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int column = 0; column < NumCols; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex h = GetHexAt(column, row);
                
                float n = Mathf.PerlinNoise(((float)column)/Mathf.Max(NumCols, NumRows)/noiseResolution + noiseOffset.x, ((float)row)/Mathf.Max(NumCols, NumRows)/noiseResolution + noiseOffset.y) - 0.5f;
                h.Elevation += n * noiseScale;
            }
        }

        noiseResolution = 0.01f;
        noiseScale = 2f;

        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int column = 0; column < NumCols; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex h = GetHexAt(column, row);

                float n = Mathf.PerlinNoise(((float)column) / Mathf.Max(NumCols, NumRows) / noiseResolution + noiseOffset.x, ((float)row) / Mathf.Max(NumCols, NumRows) / noiseResolution + noiseOffset.y) - 0.5f;
                h.Moisture = n * noiseScale;
            }
        }



        UpdateHexVisuals();
    }


    void ElevateArea(int q, int r, int radius, float centerHeight = 1f)
    {
        radius++;
        Hex centerHex = GetHexAt(q, r);
        
        Hex[] areaHexes = GetHexesInRadius(centerHex, radius);

        foreach(Hex h in areaHexes)
        {
            
            h.Elevation =  centerHeight * Mathf.Lerp(1f, 0.25f, Mathf.Pow(HexGrid.DistanceBetweenHexes(centerHex, h)/radius, 2));
        }
        
    }

   
}
