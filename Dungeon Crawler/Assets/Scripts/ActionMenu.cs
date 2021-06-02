using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
* Classe dos menus utilizados nas batalhas para input do jogador
*/
public class ActionMenu : MonoBehaviour
{
    private const int BAGSIZE = 60;//tamanho maximo da mochila
    public List<GameObject> SubMenu = new List<GameObject>();
    public GameObject MainMenu;
    public Button defaultButton;

    void Update()
    {
        if (Input.GetKeyDown("x") && MainMenu != null) //Returns to father menu
        {
            OnMainMenuButton();
        }
    }

    public void OpenMenu(){
        this.transform.GetChild(0).gameObject.SetActive(true);//abre main menu
    }

    void OnEnable()
    {
        StartCoroutine(SelectDefaultButton());
    }
    /**
    * Seleciona o primeiro botão do menu
    */
    IEnumerator SelectDefaultButton()
    {
        yield return null;
        defaultButton.Select();// por algum motivo o botao nao fica em highlight quando o menu é ativado pelo 
                               // SelectNextPartyMember(), verificar porquê e consertar algum dia.
    }

    /**
    * Função chamada ao se clicar o botão associado
    * Abre o SubMenu
    */
    public void OnSubMenuButton(int index)
    {
        SubMenu[index].SetActive(true);
        this.gameObject.SetActive(false);
    } 

    /**
    * Função chamada ao se clicar o botão associado
    * Fecha o SubMenu e abre o Menu Principal
    */ 
    public void OnMainMenuButton()
    {
        MainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
