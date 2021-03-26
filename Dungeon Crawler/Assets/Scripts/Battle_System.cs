using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, MOVESELECTIONTURN, TARGETSELECTIONTURN, BATTLETURN, WON, LOST }

public class Battle_System : MonoBehaviour
{
    private const int BAGSIZE = 60;//tamanho maximo da mochila 

    //Indica qual unidade está selecionada para seleção de ação
    int SelectedPartyMember = 0;

    int SelectedEnemy = 0;

    public BattleState state;

    public GameObject partyPrefab;
    public GameObject enemyPrefab;
    public Transform enemyBattleStation;

    public Text dialogueText;

    public GameObject partyHUD;

    PlayerBattleHUD[] partyMemberHUDs = new PlayerBattleHUD[6];

    Unit[] partyMembers = new Unit[6];

    Unit[] enemyUnits = new Unit[6];

    public GameObject ActionMenu;

    private List<Unit> MaxPriorityBattleOrder = new List<Unit>();//nessa lista serão guardados unidades cuja ação é de prioridade maxima,
                                             // por enquanto só Guard tem esse tipo de prioridade já que é uma ação que deve ocorrer antes de qualquer outra
    private List<Unit> PriorityBattleOrder = new List<Unit>(); //Nessa lista vai ficar todos os personagens que usarem habilidades com prioridade

    private List<Unit> BattleOrder = new List<Unit>(); //Nessa lista serão colocados todas as unidades por ordem de velocidade,
                                   //de ser feito um sorting por ordem de velocidade antes de ser passado para
                                   //a função que executa as ações no turno de batalha
    private List<Unit> LowPriorityBattleOrder = new List<Unit>(); //Lista para unidade que executam movimentos de baixa prioridade no seu turno 

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    public void Update()
    {
        if(state == BattleState.TARGETSELECTIONTURN){//deals with the inputs during TargetSelectionTurn
            if (Input.GetButtonDown("Submit"))//isso aqui ta insta sendo ativado quando entra no TARGETSELECTIONTURN
            {
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, new List<Unit> {enemyUnits[SelectedEnemy]});
                enemyUnits[SelectedEnemy].IsSelected(false);
                StartCoroutine(SelectNextPartyMember());
            }
            // if (Input.GetButtonDown("Direita"))
            // {
            //     StartCoroutine(SelectNextPartyMember());
            // }
            // if (Input.GetButtonDown("Esquerda"))
            // {
            //     StartCoroutine(SelectNextPartyMember());
            // }
        }
        
    }

    IEnumerator SetupBattle(){
        GameObject partyGO = Instantiate(partyPrefab);
        for(int i = 0; i < 6; i++){
            if (partyGO.transform.GetChild(i).GetComponent<Unit>().unitName == "")//por enquanto estou considerando que se a unidade nao tiver nome, ela nao existe
                continue;
            partyMembers[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            partyMemberHUDs[i] = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            partyMemberHUDs[i].SetHUD(partyMembers[i]);
        }
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);//por enquanto soh tem 1 inimigo
        enemyUnits[0] = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A wild demon horde has appeared!";

        yield return new WaitForSeconds(2f);

        MoveSelectionTurn();
    }
    //todas funçoes que mudam o estado do state devem ser Coroutines para evitar bugs
    void MoveSelectionTurn(){
        dialogueText.text = "What will you do?";
        //At the start of the MOveSelectionTurn, input the AI moves
        foreach (Unit unit in enemyUnits)
        {
            if(!unit){
                continue;
            }
            switch (unit.AI.EnemyMove(partyMembers, enemyUnits))
            {
                case 0:
                    LowPriorityBattleOrder.Add(unit);
                break;
                case 1:
                    BattleOrder.Add(unit);
                break;
                case 2:
                    PriorityBattleOrder.Add(unit);
                break;
                case 3:
                    MaxPriorityBattleOrder.Add(unit);
                break;
                default:
                break;
            }
        }
        StartCoroutine(SelectFirstPartyMember());
    }

    IEnumerator BattleTurn(){
        state = BattleState.BATTLETURN;

        print("This is the battleturn\n");

        yield return null;//waits 1 frame
        //sort BattleOrder lists based on user speed and speedmodifiers
        //SortLists();
        foreach (Unit unit in MaxPriorityBattleOrder)//First play all MaxPriority moves in order of user speed
        {
            unit.Move.PerformAction();
        }
        MaxPriorityBattleOrder.Clear();
        
        foreach (Unit unit in PriorityBattleOrder)//Second play all Priority moves in order of user speed
        {
            unit.Move.PerformAction();
        }
        PriorityBattleOrder.Clear();

        foreach (Unit unit in BattleOrder)//Third play all normal Priority moves in order of user speed
        {
            unit.Move.PerformAction();
        }
        BattleOrder.Clear();

        foreach (Unit unit in LowPriorityBattleOrder)//Last play all low Priority moves in order of user speed
        {
            unit.Move.PerformAction();
        }
        LowPriorityBattleOrder.Clear();

        //End Battle Turn, return to moveselectionturn
        MoveSelectionTurn();
    }

    IEnumerator TargetSelectionTurn(){//talvez se deva passar algum parametro aqui que indique se a ação acerta mais de 1 alvo
        yield return null;//waits 1 frame
        state = BattleState.TARGETSELECTIONTURN;
        foreach (Transform child in ActionMenu.transform)//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        enemyUnits[SelectedEnemy].IsSelected(true);//por enquanto soh tem 1 inimigo
    }

    IEnumerator SelectFirstPartyMember(){
        state = BattleState.MOVESELECTIONTURN;
        while (!partyMembers[SelectedPartyMember] && SelectedPartyMember < 6){
            SelectedPartyMember++;
        }
        if (SelectedPartyMember >= 6){
            print("alguma coisa deu errado");
        }
        else{
            partyMemberHUDs[SelectedPartyMember].is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    //seleciona proximo membro da equipe para seleção de Ação
    IEnumerator SelectNextPartyMember(){
        state = BattleState.MOVESELECTIONTURN;
        partyMemberHUDs[SelectedPartyMember].is_Selected(false); 
        while (!partyMembers[SelectedPartyMember++] && SelectedPartyMember < 6);
        if (SelectedPartyMember >= 6){
            SelectedPartyMember = 0;
            print("Entrando no BattleFase\n");
            StartCoroutine(BattleTurn());
        }
        else{
            partyMemberHUDs[SelectedPartyMember].is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    void SelectNextEnemy(){//to do
        // partyMemberHUDs[SelectedPartyMember].is_Selected(false); 
        // while (partyMembers[SelectedPartyMember++].unitName == "" && SelectedPartyMember < 6);
        // if (SelectedPartyMember >= 6){
        //     SelectedPartyMember = 0;
        //     state = BattleState.BATTLETURN;//inicia o estado de batalha
        //     print("Entrando no BattleFase\n");
        //     BattleTurn();
        // }
        // else{
        //     partyMemberHUDs[SelectedPartyMember].is_Selected(true); 
        //     ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        // }
    }

    //Seleciona o Membro anterior para refazer sua ação
    IEnumerator SelectPreviousPartyMember(){
        state = BattleState.MOVESELECTIONTURN;
        partyMemberHUDs[SelectedPartyMember].is_Selected(false); 
        int Atual = SelectedPartyMember;
        while (!partyMembers[SelectedPartyMember] && SelectedPartyMember >= 0)
        {
            SelectedPartyMember--;
        }
        if (SelectedPartyMember < 0){
            SelectedPartyMember = Atual;
        }
        else{
            partyMemberHUDs[SelectedPartyMember].is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    void SelectPreviousEnemy(){// to do
        // partyMemberHUDs[SelectedPartyMember].is_Selected(false); 
        // int Atual = SelectedPartyMember;
        // while (partyMembers[SelectedPartyMember].unitName == "" && SelectedPartyMember >= 0)
        // {
        //     SelectedPartyMember--;
        // }
        // if (SelectedPartyMember < 0){
        //     SelectedPartyMember = Atual;
        // }
        // else{
        //     partyMemberHUDs[SelectedPartyMember].is_Selected(true); 
        //     ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        // }
    }


    public void OnActionButton(int index) //tem que mudar essa parte aqui para poder selecionar o alvo
                                          // quando puder selecionar o alvo, tem que passar a ação só depois de 
                                          // escolher o alvo
                                          //Entao teria que ser: "OnActionButton registra a ação e inicia a
                                          // rotina de escolher o alvo?"
                                          //Como seria para cancelar essa ação?
    {
        if (state != BattleState.MOVESELECTIONTURN){
            return;
        }
        switch (index)
        {
            case 0:{//Attack
                //registra ataque e alvo
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.ATTACK);//registra a ação
                BattleOrder.Add(partyMembers[SelectedPartyMember]);// a ação de atacar é uma ação de prioridade normal

                StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
            }
            break;

            case 1:{//Guard
                //registra a ação de defesa
                //e como o target é a propria unidade que esta usando, ja seleciona o proximo membro do grupo para seleçao de ação
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.GUARD, new List<Unit>() {partyMembers[SelectedPartyMember]});
                MaxPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);//coloca a unidade na lista de prioridade adequada
                StartCoroutine(SelectNextPartyMember());
            }
            break;

            case 2:{//Escape
                //registra ataque e alvo e depois seleciona proximo membro para registrar o movimento
                StartCoroutine(SelectNextPartyMember());
            }
            break;

            default:
            break;
        }
    }
}
