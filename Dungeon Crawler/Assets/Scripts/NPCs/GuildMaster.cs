using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildMaster : MonoBehaviour
{
    public GameObject menu;
    public GameObject partyHUD;
    private NpcInteractionSystem npcInteractionSystem;
    private bool heal = false;
    private DialogueManager dialogueManager;
    private Unit[] party = new Unit[6];
    public ScrollMenuContent scrollMenuContent;
    public string npc_name = "Master Bertrand";
    private int SelectedPartyMember = 0;
    private bool onDemonSelect = false;
    private bool VerticalPressed = false;
    private int MAX_UNIT_SIZE = 6;
    public GameObject giftUnits;
    private Unit chosenUnit;
    public Stat_Screen statScreen;
    void Start(){
        GameManager.Instance.Init();
        if(!GameManager.Instance.EventList[(int)GameManager.Event.GotFirstDemon]){
            menu.transform.GetChild(0).gameObject.SetActive(true);
        }
        else{
            menu.transform.GetChild(3).gameObject.SetActive(true);
        }
        npcInteractionSystem = FindObjectOfType<NpcInteractionSystem>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        SetupGUI();
        foreach (Unit unit in giftUnits.GetComponentsInChildren<Unit>())
        {
            unit.InitStats();
        }
    }

    void Update(){
    }

    public void CloseMenu(){
        menu.SetActive(false);
    }
    public void OnDemonChoiceButton(Unit unit){
        chosenUnit = unit;
        menu.SetActive(false);
        dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                      new string[1] {"Are you sure you want " + chosenUnit.unitName+"?"},
                                                    1f));
    }

    public void ReceiveDemon(){
        GameManager.Instance.CapturedUnit(chosenUnit);
        HideDemonInformation();
        dialogueManager.StartDialogue(new Dialogue(npc_name, 
                                      new string[4] {
                                    "That was a good choice, " + chosenUnit.unitName + "it’s a great demon.",
                                    "Since I’m in a really good mood, you can have this 10 Demon Sealers so you can capture your on demons, and this Demonomicon so you can store demons that you are not currently using.",
                                    "And here is the package that you need to deliver. If you can deliver it safely, there will be more jobs for you.",
                                    "..."
                                      },
                                                    1f));
        GameManager.Instance.EventList[(int)GameManager.Event.GotFirstDemon] = true;
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

    /**
    * Abre a janela de informações do demonio atualmente selecionado
    */
    public void ShowDemonInformation(Unit u){
        statScreen.gameObject.SetActive(true);
        statScreen.UpdateStatScreen(u);
    }

    /**
    * Abre a janela de informações de demonios
    */
    public void HideDemonInformation(){
        statScreen.gameObject.SetActive(false);
    }

}
