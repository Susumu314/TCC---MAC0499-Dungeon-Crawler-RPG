using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demonologist : MonoBehaviour
{
    public enum ACTIONS {HEAL, SUMMON, DISMISS}
    public GameObject menu;
    public GameObject partyHUD;
    private NpcInteractionSystem npcInteractionSystem;
    private bool heal = false;
    private DialogueManager dialogueManager;
    private Unit[] party = new Unit[6];
    public ScrollMenuContent scrollMenuContent;
    public string npc_name = "Clara";
    private int SelectedPartyMember = 0;
    private bool onDemonSelect = false;
    private bool VerticalPressed = false;
    private int MAX_UNIT_SIZE = 6;
    void Start(){
        npcInteractionSystem = FindObjectOfType<NpcInteractionSystem>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        SetupGUI();
        scrollMenuContent.InitDemonomiconMenu(GameManager.Instance.party.GetComponent<Demonomicon>(), this);
        AudioManager.instance.Play("Demonologist");
    }

    void Update(){
        DemonSelection();
        if(heal){
            if(dialogueManager.dialogueFinished){
                Heal();
            }
        }
        if(partyHUD.activeSelf && dialogueManager.dialogueFinished){
            partyHUD.SetActive(false);
        }
    }
    public void OnMenuButton(int action){
        menu.SetActive(false);
        npcInteractionSystem.canOpenMenu = false;
        switch (action)
        {
            case (int)ACTIONS.HEAL:
                partyHUD.SetActive(true);
                dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                                                  new string[2] {"Okay, I'll take your Demons for a second.", 
                                                                                 "..."},
                                                                                 2f));
                heal = true;
            break;
            case (int)ACTIONS.DISMISS:
                partyHUD.SetActive(true);
                FindObjectOfType<DialogueManager>().StartDialogue(new Dialogue(npc_name, 
                                                                  new string[1] {"Which demon do you want to store in the Demonomicon?"},
                                                                                 2f));
                StartDemonSelection();
            break;
            default:
            break;
        }
    }
    public void SetupGUI(){
        GameObject partyGO = GameManager.Instance.party;
        for(int i = 0; i < 6; i++){
            if (partyGO.transform.GetChild(i).GetComponent<Unit>().species == ""){//por enquanto estou considerando que se a unidade nao tiver nome, ela nao existe
                partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>().Reset();
                continue;
            }
            party[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            party[i].InitStats();
            party[i].ResetStatMods(); 
            party[i].isBackLine = i/3;
            party[i].HUD = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            party[i].HUD.SetHUD(party[i]);
        }
    }

    private void Heal(){
        foreach (Unit unit in party)
        {
            if(unit){
                if(unit.species != ""){
                    unit.FullHeal();
                }
            }
        }
        FindObjectOfType<DialogueManager>().StartDialogue(new Dialogue(npc_name, 
                                                                  new string[2] {"Thanks for waiting", 
                                                                                 "There is anything else?"},
                                                                                 2f));
        heal = false;
        npcInteractionSystem.canOpenMenu = true;
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnSummonDemonButton(Demonomicon.SavedDemon demon)  //Como seria para cancelar essa ação?
    {
        menu.SetActive(false);
        partyHUD.SetActive(true);
        bool achou = false;
        int i = 0;
        Unit unit;
        unit = GameManager.Instance.party.transform.GetChild(i).GetComponent<Unit>();
        while(!achou && i < 6){
            unit = GameManager.Instance.party.transform.GetChild(i).GetComponent<Unit>();
            if(unit.species == ""){
                achou = true;
                continue;
            }
            i++;
        }
        if (achou){
            unit.Summon(demon.SPECIES, demon.totalExp, demon.nickname, demon.skills, i/3);
            unit.isPlayerUnit = true;
            FindObjectOfType<DialogueManager>().StartDialogue(
            new Dialogue(npc_name, 
            new string[2] {"Summoning " + demon.SPECIES,
                           "Do you want to summon another?"},
            2f));
            party[i] = GameManager.Instance.party.transform.GetChild(i).GetComponent<Unit>();
            party[i].HUD = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            party[i].HUD.SetHUD(party[i]);
            scrollMenuContent.DeleteDemonomiconEntry(demon);
            return;
        }
        FindObjectOfType<DialogueManager>().StartDialogue(
            new Dialogue(npc_name, 
            new string[1] {"You can't carry more than 6 summoned demons, please dismiss at least one to make room for a new one."},
            2f)); 
    }

    /**
    * Inicia o estado de selecionar a unidade aliada para utilizar os comandos do party menu
    */
    public void StartDemonSelection(){
        StartCoroutine(SelectFirstPartyMember());
    }

    /**
    * Procura e seleciona o primeiro membro da equipe do jogador que está vivo para seleção de ação.
    */
    IEnumerator SelectFirstPartyMember(){
        SelectedPartyMember = 0;
        while ((!party[SelectedPartyMember] || party[SelectedPartyMember].isDead)
                && SelectedPartyMember < 6){
            SelectedPartyMember++;
        }
        if (SelectedPartyMember >= 6){
            print("alguma coisa deu errado");
        }
        else{
            party[SelectedPartyMember].HUD.is_Selected(true); 
        }
        yield return new WaitForEndOfFrame();//waits 1 frame
        onDemonSelect = true;
    }

    private void DemonSelection(){
        if(onDemonSelect){
            if (Input.GetButtonDown("Submit"))
            {
                if(LastHealthyDemon(SelectedPartyMember)){ // checa se esta querendo depositar o ultimo demonio com vida acima de 0
                    dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                                                  new string[1] {"You cannot store you last healthy demon!"},
                                                                                 2f));
                    npcInteractionSystem.canOpenMenu = true;
                    onDemonSelect = false;
                    return;
                }
                scrollMenuContent.AddDemonomiconEntry(GameManager.Instance.party.GetComponent<Demonomicon>().AddDemon(party[SelectedPartyMember]));
                party[SelectedPartyMember].HUD.Reset();
                party[SelectedPartyMember].species = "";
                party[SelectedPartyMember] = null;
                FindObjectOfType<DialogueManager>().StartDialogue(new Dialogue(npc_name, 
                                                                  new string[2] {"Sealing Demon on Demonomicon", 
                                                                                 "What else can I help you?"},
                                                                                 2f));
                npcInteractionSystem.canOpenMenu = true;
                onDemonSelect = false;
            }
            else if(Input.GetButtonDown("Cancel")){//volta ao menu principal
                npcInteractionSystem.canOpenMenu = true;
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
    * Seleciona o proximo aliado que está vivo para seleção de alvo.
    */
    IEnumerator SelectNextPartyMember(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        party[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember++;
        if(SelectedPartyMember >= party.Length)
                SelectedPartyMember = 0;
        //Loop to get an alive unit, assumes that there is at least 1 alive unit
        while(!party[SelectedPartyMember] || party[SelectedPartyMember].isDead){
            SelectedPartyMember++;
            if(SelectedPartyMember >= MAX_UNIT_SIZE)
                SelectedPartyMember = 0;
        }
        party[SelectedPartyMember].HUD.is_Selected(true); 
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
        party[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember += 3;
        if(SelectedPartyMember >= party.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!party[SelectedPartyMember] || party[SelectedPartyMember].isDead){
            if (frontRow)
                SelectedPartyMember = 3;
            else
                SelectedPartyMember = 0;
        }
        while(!party[SelectedPartyMember] || party[SelectedPartyMember].isDead){
            SelectedPartyMember++;
            if(SelectedPartyMember >= party.Length)
                SelectedPartyMember -= MAX_UNIT_SIZE;
        }
        party[SelectedPartyMember].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o aliado anterior que está vivo para seleção de ação
    */
    IEnumerator SelectPreviousPartyMember(){// to do
        party[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember--;
        if(SelectedPartyMember < 0)
                SelectedPartyMember = party.Length - 1;
        //Loop to get an alive unit
        while(!party[SelectedPartyMember] || party[SelectedPartyMember].isDead){
            SelectedPartyMember--;
            if(SelectedPartyMember < 0)
                SelectedPartyMember = party.Length - 1;
        }
        party[SelectedPartyMember].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }

    /**
    * Checa se tem mais de um membro na equipe
    */
    public bool LastHealthyDemon(int unitId){
        for (int i = 0; i < 6; i++)
        {
            if(i == unitId){
                continue;
            }
            if(party[i] && !party[i].isDead){
                return false;
            }
        }
        return true;
    }

}
