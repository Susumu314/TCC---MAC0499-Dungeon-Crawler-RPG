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
    public Battle_System Battle_System;

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) //Returns to father menu
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
        if(Battle_System != null)
            Battle_System.dialogueText.text = "What will you do?";
    }
    /**
    * Seleciona o primeiro botão do menu
    */
    IEnumerator SelectDefaultButton()
    {
        yield return null;
        if (defaultButton == null){//se o botao default da mochila foi destruido, setar o primeiro como default
            defaultButton = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>();
        }
        defaultButton.Select();// por algum motivo o botao nao fica em highlight quando o menu é aberto
                            // depois de ser selecionado o default button
        defaultButton.OnSelect(null); 
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

    /**
    * Inicializa referencia ao Battle System
    */
    public void InitBS(Battle_System b){
        Battle_System = b;
    }
}
