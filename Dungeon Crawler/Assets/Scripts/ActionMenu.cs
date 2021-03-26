using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    private const int BAGSIZE = 60;//tamanho maximo da mochila
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
        defaultButton.Select();// por algum motivo o botao nao fica em highlight quando o menu é ativado pelo 
                               // SelectNextPartyMember(), verificar porquê e consertar algum dia.
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
