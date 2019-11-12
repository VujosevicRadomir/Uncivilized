using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonHandler : MonoBehaviour {

    public GameObject IGMenu;

	public void ShowIGMenu()
    {
        IGMenu.SetActive(true);
        StateManager.IGMenuOpen = true;
    }

    public void HideIGMenu()
    {
        IGMenu.SetActive(false);
        StateManager.IGMenuOpen = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
