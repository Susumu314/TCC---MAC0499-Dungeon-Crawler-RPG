using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



enum Target
{
    ENEMY_0, ENEMY_1, ENEMY_2, ENEMY_3, ENEMY_4, ENEMY_5,
    NULL, FRONTROW, BACKROW, ALL,
    ALLY_0, ALLY_1, ALLY_2, ALLY_3, ALLY_4, ALLY_5,
    PARTY_FRONTROW, PARTY_BACKROW, PARTY_ALL
}

// struct Action //Toda ação será uma struct com o ato do personagem e o alvo
// {
//     public Act act;
//     public Target target;
// }


public enum BattleState { START, STRATEGYTURN, BATTLETURN, WON, LOST }

public class Battle_System : MonoBehaviour
{
    int SelectedPartyMember = 0;

    //Action[] PartyMemberAction = new Action[6]; // action taken by PartyMember[i] on last turn
    public BattleState state;

    public GameObject partyPrefab;
    public GameObject enemyPrefab;
    public Transform enemyBattleStation;

    public Text dialogueText;

    public GameObject partyHUD;

    PlayerBattleHUD[] partyMemberHUDs = new PlayerBattleHUD[6];

    Unit[] partyMembers = new Unit[6];
    Unit enemyUnit;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle(){
        GameObject partyGO = Instantiate(partyPrefab);
        for(int i = 0; i < 6; i++){
            partyMembers[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            partyMemberHUDs[i] = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            partyMemberHUDs[i].SetHUD(partyMembers[i]);
        }
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A wild demon horde has appeared!";

        yield return new WaitForSeconds(2f);
        
        state = BattleState.STRATEGYTURN;
        StrategyTurn();
    }

    void StrategyTurn(){
        dialogueText.text = "What will you do?";
    }

    public void OnActionButton(int index) //tem que mudar essa parte aqui para poder selecionar o alvo
                                          // quando puder selecionar o alvo, tem que passar a ação só depois de 
                                          // escolher o alvo
                                          //Entao teria que ser: "OnActionButton registra a ação e inicia a
                                          // rotina de escolher o alvo?"
                                          //Como seria para cancelar essa ação?
    {
        if (state != BattleState.STRATEGYTURN){
            return;
        }
        switch (index)
        {
            case 0://attack
                // StartCoroutine(PartyAction(Act.ATTACK, Target.ENEMY_0));
            break;

            default:
            break;
        }
    }

    // IEnumerator PartyAction(Act act, Target target){
    //     PartyMemberAction[SelectedPartyMember].act = act;
    //     PartyMemberAction[SelectedPartyMember].target = target;
    //     SelectedPartyMember += 1; 
    //     yield return new WaitForSeconds(0.5f);
    //     if (SelectedPartyMember > 5){
    //         SelectedPartyMember = 0;
    //         state = BattleState.BATTLETURN;
    //     }
    // }
}
