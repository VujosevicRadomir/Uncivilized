using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class StateManager {

    public static bool IGMenuOpen = false;

    private static Hex selectedHex = null;
    public static Unit SelectedUnit;

    public static Hex SelectedHex {
        get
        {
            return selectedHex;
        }
        set
        {
            if(value == null)
            {
                selectedHex = null;
                HexSelected = false;
                return;
            }
            if(value.units == null || value.units.Count == 0)
            {
                Debug.Log("Can't select a Hex without units");
                return;
            }

            SelectedUnit = value.units.First();

            HexSelected = true;
            selectedHex = value;
        }
    }
    public static bool HexSelected = false;
}
