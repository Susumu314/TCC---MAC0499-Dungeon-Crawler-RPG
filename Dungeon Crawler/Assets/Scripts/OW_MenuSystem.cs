using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OW_MenuSystem : MonoBehaviour
{
    private enum OW_State { FREEROAMING, MOVESELECTION, DEMONSELECTION, TARGETSELECTION }
    private OW_State state;
    public Text dialogueText;
    public GameObject partyHUD;
    private Unit[] partyMembers = new Unit[MAX_UNIT_SIZE]; 
    private const int BAGSIZE = 60;//tamanho maximo da mochila 
    private const int MAX_UNIT_SIZE = 6;   
    int SelectedPartyMember = 0;
    int TargetPartyMember = 0;
    public Bag bag;
    private ScrollMenuContent bagMenuContent;
    public GameObject ActionMenu;
    private Skill.TARGET_TYPE targetMode = Skill.TARGET_TYPE.SINGLE_ALLY;
    // Start is called before the first frame update
    void Start()
    {
        state = OW_State.FREEROAMING;
        SetupGUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupGUI(){
        bagMenuContent = ActionMenu.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<ScrollMenuContent>();
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
        }
        bagMenuContent.InitBagMenu(bag, this);
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnItemButton(int index)  //Como seria para cancelar essa ação?
    {
        if (state != OW_State.MOVESELECTION){
            return;
        }
        foreach (Transform child in ActionMenu.transform){//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        }
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
}
