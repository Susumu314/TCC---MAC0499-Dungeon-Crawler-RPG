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
    void Start(){
        npcInteractionSystem = FindObjectOfType<NpcInteractionSystem>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        SetupGUI();
        scrollMenuContent.InitDemonomiconMenu(GameManager.Instance.party.GetComponent<Demonomicon>(), this);
    }

    void Update(){
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
                FindObjectOfType<DialogueManager>().StartDialogue(new Dialogue("DemonologistName", 
                                                                  new string[2] {"Okay, I'll take your Demons for a second.", 
                                                                                 "..."},
                                                                                 2f));
                heal = true;
            break;
            case (int)ACTIONS.SUMMON:

            break;
            case (int)ACTIONS.DISMISS:
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
        FindObjectOfType<DialogueManager>().StartDialogue(new Dialogue("DemonologistName", 
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
    public void OnSavedDemonButton(int index)  //Como seria para cancelar essa ação?
    {
        //if (state != BattleState.MOVESELECTIONTURN){
        //    return;
        //}
        foreach (Transform child in menu.transform){//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        }
        //SummonTarget = demonomicon[index];
        //Iniciar etapa de procura pelo local aonde vai invocar o demonio
        // quando escolhe a posicao onde invocar, invoca o demonio
        //targetMode = (Skill.TARGET_TYPE)a[1];
        //StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
        
    }
}
