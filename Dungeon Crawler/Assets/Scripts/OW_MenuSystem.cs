﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OW_MenuSystem : MonoBehaviour
{
    public enum OW_State { FREEROAMING, ONMAINMENU, DEMONSELECTION, TARGETSELECTION, PERFORMINGACTION, DEMONMENU}
    public OW_State state;
    public Text dialogueText;
    public GameObject partyHUD;
    private Unit[] partyMembers = new Unit[MAX_UNIT_SIZE]; 
    private const int BAGSIZE = 60;//tamanho maximo da mochila 
    private const int MAX_UNIT_SIZE = 6;   
    int SelectedPartyMember;
    int TargetPartyMember;
    public Bag bag;
    private ScrollMenuContent bagMenuContent;
    public GameObject ActionMenu;
    private Skill.TARGET_TYPE targetMode = Skill.TARGET_TYPE.SINGLE_ALLY;
    private bool VerticalPressed = false;
    private bool DirectionalPressed = false;
    private bool onDemonMenu = false;
    // Start is called before the first frame update
    void Start()
    {
        state = OW_State.FREEROAMING;
        SelectedPartyMember = 0;
        TargetPartyMember = 0;
        SetupGUI();
        while ((!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead)
                && SelectedPartyMember < 6){
            SelectedPartyMember++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("OpenMenu")){
            OpenMenu();
        }
        TargetSelection();
        DemonSelection();
        PartyMenu();
    }

    /**
    * Quando fechar o partymenu, voltar para a etapa de seleção de unidade
    */
    private void PartyMenu(){
        if(ActionMenu.transform.GetChild(1).gameObject.activeSelf){
            if(Input.GetButtonDown("Cancel")){
                StartDemonSelection();
            }
        }
    }

    /**
    * Função que abre e fecha o menu quando no overworld
    */
    public void OpenMenu(){
        if(state == OW_State.FREEROAMING){
            state = OW_State.ONMAINMENU;
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);
        }
        else{
            state = OW_State.FREEROAMING;
            CloseAll();
        }
    }

    /**
    * Função que fecha todos o menus e abas abertas para retornar ao freeroaming
    */
    private void CloseAll(){
        ActionMenu.transform.GetChild(0).gameObject.SetActive(false);
    }


    public void SetupGUI(){
        bagMenuContent = ActionMenu.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<ScrollMenuContent>();
        GameObject partyGO = GameManager.Instance.party;
        bag = GameManager.Instance.party.GetComponent<Bag>();
        for(int i = 0; i < 6; i++){
            if (partyGO.transform.GetChild(i).GetComponent<Unit>().species == "")//por enquanto estou considerando que se a unidade nao tiver nome, ela nao existe
                continue;
            partyMembers[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            partyMembers[i].InitStats();
            partyMembers[i].ResetStatMods(); 
            partyMembers[i].isBackLine = i/3;
            partyMembers[i].HUD = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            partyMembers[i].HUD.SetHUD(partyMembers[i]);
            partyMembers[i].MenuSystemReference(this);
        }
        bagMenuContent.InitBagMenu(bag, this);
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnItemButton(int index)  //isso aqui tem que mudar
    {
        Debug.Log(index);
        if (state != OW_State.ONMAINMENU){
            return;
        }
        ActionMenu.SetActive(false);
        int[] a = partyMembers[SelectedPartyMember].Move.SetItem(index);//registra a ação
        targetMode = (Skill.TARGET_TYPE)a[1];
        StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
    }

    /**
    * @brief Etapa de seleção de alvo para habilidades e ataques.
    * Após o jogador selecionar uma ações, o turno de seleção de alvo permite selecionar quais 
    * unidades serão o alvo da ação
    */
    IEnumerator TargetSelectionTurn(){
        yield return null;
        dialogueText.text = "Select Target";
        state = OW_State.TARGETSELECTION;
        //Loop to get an alive unit
        if (targetMode == Skill.TARGET_TYPE.SINGLE_ALLY){
            while (!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
                TargetPartyMember++;
                if (TargetPartyMember >= MAX_UNIT_SIZE){
                    TargetPartyMember = 0;
                }
            }
            partyMembers[TargetPartyMember].HUD.is_Target(true); 
        }

        else if (targetMode == Skill.TARGET_TYPE.SELF){
            TargetPartyMember = SelectedPartyMember;
            partyMembers[TargetPartyMember].HUD.is_Target(true); 
        }

        else if (targetMode == Skill.TARGET_TYPE.ALLY_ROW){
            while(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
                TargetPartyMember++;
                if(TargetPartyMember >= partyMembers.Length)
                    TargetPartyMember = 0;
            }
            if(TargetPartyMember <  3){
                for (int i = 0; i < 3; i++)
                {
                    if(partyMembers[i] && !partyMembers[i].isDead){
                        partyMembers[i].HUD.is_Target(true);
                    }
                }
            }
            else{
                for (int i = 3; i < 6; i++)
                {
                    if(partyMembers[i] && !partyMembers[i].isDead){
                        partyMembers[i].HUD.is_Target(true);
                    }
                }
            }
        }
    }

    private void TargetSelection(){
        if(state == OW_State.TARGETSELECTION){//deals with the inputs during TargetSelectionTurn,
            if(targetMode == Skill.TARGET_TYPE.SINGLE_ALLY){
                if (Input.GetButtonDown("Submit"))
                {
                    partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, new List<Unit> {partyMembers[TargetPartyMember]});
                    partyMembers[TargetPartyMember].HUD.is_Target(false);
                    StartCoroutine(PerformAction());
                }
                else if (Input.GetButtonDown("Direita"))
                {
                    StartCoroutine(TargetNextPartyMember());
                }
                else if (Input.GetButtonDown("Esquerda"))
                {
                    StartCoroutine(TargetPreviousPartyMember());
                }
                else if(Input.GetAxisRaw("Vertical") != 0)
                {
                    if(!VerticalPressed)
                    {
                        // Call your event function here.
                        VerticalPressed = true;
                        StartCoroutine(TargetNextRowPartyMember());
                    }
                }
                else if( Input.GetAxisRaw("Vertical") == 0)
                {
                    VerticalPressed = false;
                } 
            }
            // Espero que isso aqui esteja certo
            else if(targetMode == Skill.TARGET_TYPE.SELF){
                if (Input.GetButtonDown("Submit"))
                {
                    partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, new List<Unit> {partyMembers[TargetPartyMember]});
                    partyMembers[TargetPartyMember].HUD.is_Target(false);
                    StartCoroutine(PerformAction());
                }
            }

            else if(targetMode == Skill.TARGET_TYPE.ALLY_ROW){
               if (Input.GetButtonDown("Submit"))//isso aqui ta insta sendo ativado quando entra no TARGETSELECTIONTURN
                {
                    List<Unit> targetList = new List<Unit>();
                    for (int i = 0; i < 6; i++)
                    {
                        if(partyMembers[i] && partyMembers[i].HUD.isTarget){
                            targetList.Add(partyMembers[i]);
                            partyMembers[i].HUD.is_Target(false);
                        }
                    }
                    partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, targetList);
                    StartCoroutine(PerformAction());
                }
                else if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
                {
                    if(!DirectionalPressed)
                    {
                        DirectionalPressed = true;
                        StartCoroutine(TargetNextPartyRow());
                    }
                }
                else if( Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
                {
                    DirectionalPressed = false;
                }
            }
        }
    }

    private void DemonSelection(){
        if(state == OW_State.DEMONSELECTION){
            if (Input.GetButtonDown("Submit"))
            {
                OpenDemonMenu();
            }
            else if(Input.GetButtonDown("Cancel")){//volta ao menu principal
                state = OW_State.ONMAINMENU;
                ActionMenu.transform.GetChild(1).gameObject.SetActive(false);
                ActionMenu.transform.GetChild(0).gameObject.SetActive(true);
                ActionMenu.SetActive(true);
                partyMembers[SelectedPartyMember].HUD.is_Selected(false);
                onDemonMenu = false;
            }
            else if (Input.GetButtonDown("Direita"))
            {
                StartCoroutine(SelectNextPartyMember());
            }
            else if (Input.GetButtonDown("Esquerda"))
            {
                StartCoroutine(SelectPreviousPartyMember());
            }
            else if(Input.GetAxisRaw("Vertical") != 0)
            {
                if(!VerticalPressed)
                {
                    VerticalPressed = true;
                    StartCoroutine(SelectNextRowPartyMember());
                }
            }
            else if( Input.GetAxisRaw("Vertical") == 0)
            {
                VerticalPressed = false;
            } 
        }
    }
    /**
    * Seleciona executa a ação selecionada no menu
    */
    private IEnumerator PerformAction(){
        state = OW_State.PERFORMINGACTION;
        yield return partyMembers[SelectedPartyMember].Move.PerformAction();
        ActionMenu.SetActive(true);
        bagMenuContent.UpdateButtons(bag);
        if(onDemonMenu){
            state = OW_State.DEMONMENU;
        }
        else{
            state = OW_State.ONMAINMENU;
        }
    }
    /**
    * Procura e seleciona o primeiro membro da equipe do jogador que está vivo para seleção de ação.
    */
    IEnumerator SelectFirstPartyMember(){
        while ((!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead)
                && SelectedPartyMember < 6){
            SelectedPartyMember++;
        }
        if (SelectedPartyMember >= 6){
            print("alguma coisa deu errado");
        }
        else{
            partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
        }
        yield return new WaitForEndOfFrame();//waits 1 frame
        state = OW_State.DEMONSELECTION;
        onDemonMenu = true;
    }

    /**
    * Seleciona o proximo aliado que está vivo para seleção de alvo.
    */
    IEnumerator TargetNextPartyMember(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        partyMembers[TargetPartyMember].HUD.is_Target(false); 
        TargetPartyMember++;
        if(TargetPartyMember >= partyMembers.Length)
                TargetPartyMember = 0;
        //Loop to get an alive unit, assumes that there is at least 1 alive unit
        while(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            TargetPartyMember++;
            if(TargetPartyMember >= MAX_UNIT_SIZE)
                TargetPartyMember = 0;
        }
        partyMembers[TargetPartyMember].HUD.is_Target(true); 
        yield return null;//waits 1 frame
    }
    
    /**
    * Seleciona o proximo inimigo que está vivo para seleção de alvo, inicialmente procura em uma coluna diferente da atual.
    */
    IEnumerator TargetNextRowPartyMember(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (TargetPartyMember > 2){
            frontRow = false; 
        }
        partyMembers[TargetPartyMember].HUD.is_Target(false); 
        TargetPartyMember += 3;
        if(TargetPartyMember >= partyMembers.Length)
                TargetPartyMember -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            if (frontRow)
                TargetPartyMember = 3;
            else
                TargetPartyMember = 0;
        }
        while(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            TargetPartyMember++;
            if(TargetPartyMember >= partyMembers.Length)
                TargetPartyMember -= MAX_UNIT_SIZE;
        }
        partyMembers[TargetPartyMember].HUD.is_Target(true); 
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o proximo linha da equipe
    */
    IEnumerator TargetNextPartyRow(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (TargetPartyMember > 2){
            frontRow = false; 
            for (int i = 3; i < 6; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Target(false);
                }
            }
        }
        else{
            for (int i = 0; i < 3; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Target(false);
                }
            }
        }

        TargetPartyMember += 3;
        if(TargetPartyMember >= partyMembers.Length)
                TargetPartyMember -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            if (frontRow)
                TargetPartyMember = 3;
            else
                TargetPartyMember = 0;
        }
        while(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            TargetPartyMember++;
            if(TargetPartyMember >= partyMembers.Length)
                TargetPartyMember -= MAX_UNIT_SIZE;
        }
        if(TargetPartyMember <  3){
            for (int i = 0; i < 3; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Target(true);
                }
            }
        }
        else{
            for (int i = 3; i < 6; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Target(true);
                }
            }
        }
        
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o aliado anterior que está vivo para seleção de alvo.
    */
    IEnumerator TargetPreviousPartyMember(){// to do
        partyMembers[TargetPartyMember].HUD.is_Target(false); 
        TargetPartyMember--;
        if(TargetPartyMember < 0)
                TargetPartyMember = partyMembers.Length - 1;
        //Loop to get an alive unit
        while(!partyMembers[TargetPartyMember] || partyMembers[TargetPartyMember].isDead){
            TargetPartyMember--;
            if(TargetPartyMember < 0)
                TargetPartyMember = partyMembers.Length - 1;
        }
        partyMembers[TargetPartyMember].HUD.is_Target(true); 
        yield return null;//waits 1 frame
    }

    /**
    * Inicia o estado de selecionar a unidade aliada para utilizar os comandos do party menu
    */

    public void StartDemonSelection(){
        ActionMenu.SetActive(false);
        StartCoroutine(SelectFirstPartyMember());
    }
    /**
    * Seleciona o proximo aliado que está vivo para seleção de alvo.
    */
    IEnumerator SelectNextPartyMember(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        partyMembers[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember++;
        if(SelectedPartyMember >= partyMembers.Length)
                SelectedPartyMember = 0;
        //Loop to get an alive unit, assumes that there is at least 1 alive unit
        while(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            SelectedPartyMember++;
            if(SelectedPartyMember >= MAX_UNIT_SIZE)
                SelectedPartyMember = 0;
        }
        partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }
    
    /**
    * Seleciona o proximo inimigo que está vivo para seleção de alvo, inicialmente procura em uma coluna diferente da atual.
    */
    IEnumerator SelectNextRowPartyMember(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (SelectedPartyMember > 2){
            frontRow = false; 
        }
        partyMembers[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember += 3;
        if(SelectedPartyMember >= partyMembers.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            if (frontRow)
                SelectedPartyMember = 3;
            else
                SelectedPartyMember = 0;
        }
        while(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            SelectedPartyMember++;
            if(SelectedPartyMember >= partyMembers.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        }
        partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o proximo linha da equipe
    */
    IEnumerator SelectNextPartyRow(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (SelectedPartyMember > 2){
            frontRow = false; 
            for (int i = 3; i < 6; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Selected(false);
                }
            }
        }
        else{
            for (int i = 0; i < 3; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Selected(false);
                }
            }
        }

        SelectedPartyMember += 3;
        if(SelectedPartyMember >= partyMembers.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            if (frontRow)
                SelectedPartyMember = 3;
            else
                SelectedPartyMember = 0;
        }
        while(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            SelectedPartyMember++;
            if(SelectedPartyMember >= partyMembers.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        }
        if(SelectedPartyMember <  3){
            for (int i = 0; i < 3; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Selected(true);
                }
            }
        }
        else{
            for (int i = 3; i < 6; i++)
            {
                if(partyMembers[i] && !partyMembers[i].isDead){
                    partyMembers[i].HUD.is_Selected(true);
                }
            }
        }
        
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o aliado anterior que está vivo para seleção de ação
    */
    IEnumerator SelectPreviousPartyMember(){// to do
        partyMembers[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember--;
        if(SelectedPartyMember < 0)
                SelectedPartyMember = partyMembers.Length - 1;
        //Loop to get an alive unit
        while(!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead){
            SelectedPartyMember--;
            if(SelectedPartyMember < 0)
                SelectedPartyMember = partyMembers.Length - 1;
        }
        partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }

    public void OpenDemonMenu(){
        state = OW_State.DEMONMENU;
        ActionMenu.SetActive(true);
        //seta as skills no submenu de skills
        GameObject skillMenu = ActionMenu.transform.GetChild(2).gameObject;
        for (int i = 0; i < 4; i++)
        {   
            int skillID = partyMembers[SelectedPartyMember].skillList[i];
            if(skillID == -1){
                skillMenu.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "Empty";
                continue;
            }
            if(!Skill.SkillList[skillID].OverworldUse){
                skillMenu.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            Skill.SkillData s = Skill.SkillList[skillID];
            skillMenu.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = s.Name;
            string description = "Power:" + Mathf.Abs(s.Power) + "  Accuracy:" + s.Accuracy;
            if(s.IsSpecial){
                description += "\nCost:" + s.Cost + " MP Type:" + s.Type;
            }      
            else{
                description += "\nCost:" + s.Cost*partyMembers[SelectedPartyMember].maxHP/100 + " HP Type:" + s.Type;
            }   
            description += "\n" + s.DESC;           
            SelectableElement selectable = skillMenu.transform.GetChild(i).GetComponent<SelectableElement>();
            selectable.text = description;
            // if(!Battle_SystemReferenceOnSkillMenu){
            //     selectable.Battle_System = this;
            // }
        }
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnSkillButton(int index)  //Como seria para cancelar essa ação?
    {
        if(partyMembers[SelectedPartyMember].skillList[index] == -1){
            return;
        }
        if (state != OW_State.DEMONMENU){
            return;
        }
        ActionMenu.SetActive(false);

        int[] a = partyMembers[SelectedPartyMember].Move.SetSkill(index);
        targetMode = (Skill.TARGET_TYPE)a[1];
        StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnSwitchButton()  //Como seria para cancelar essa ação?
    {
        if (state != OW_State.DEMONMENU){
            return;
        }
        ActionMenu.SetActive(false);

        partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.SWITCH);
        targetMode = Skill.TARGET_TYPE.SINGLE_ALLY;
        StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
    }
}
