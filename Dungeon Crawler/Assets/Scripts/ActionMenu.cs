using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    public GameObject SubMenu;
    public GameObject MainMenu;
    public Button defaultButton;

    void Update()
    {
        if (Input.GetKeyDown("x") && MainMenu != null) //Returns to father menu
        {
            OnMainMenuButton();
        }
    }

    void OnEnable()
    {
        StartCoroutine(SelectDefaultButton());
    }

    IEnumerator SelectDefaultButton()
    {
        yield return null;
        defaultButton.Select();
    }

    public void OnSubMenuButton()
    {
        SubMenu.SetActive(true);
        this.gameObject.SetActive(false);
    } 

    public void OnMainMenuButton()
    {
        MainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
