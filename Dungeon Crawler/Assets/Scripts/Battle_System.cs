using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//comparador para ordenação das listas de ordem de batalha baseada na velocidade das unidades
//ordena as listas por ordem descrescente de velocidade
class Comparer : IComparer<Unit>
{
    public int Compare(Unit x, Unit y)
    {
        if (!x || !y)
        {
            return 0;
        }
          
        // CompareTo() method
        return y.speed.CompareTo(x.speed);
          
    }
}

public enum BattleState { START, MOVESELECTIONTURN, TARGETSELECTIONTURN, BATTLETURN, WON, LOST }

public class Battle_System : MonoBehaviour
{
    private int enemy_number = 5; //numero de inimigos temporario ate decidir direito como vão ser os grupos de inimig
                                  //vai ser aleatorio o numero de inimigos e os tipos de inimigos?
                                  //vão ser pré definidos grupos de inimigos por local igual etrian odyssey?
    private const int BAGSIZE = 60;//tamanho maximo da mochila 
    private const int MAX_UNIT_SIZE = 6;

    //Indica qual unidade está selecionada para seleção de ação
    int SelectedPartyMember = 0;

    int SelectedEnemy = 0;

    public BattleState state;

    public GameObject partyPrefab;
    public GameObject enemyPrefab;
    public Transform enemyBattleStation;

    public Text dialogueText;

    public GameObject partyHUD;

    Unit[] partyMembers = new Unit[MAX_UNIT_SIZE]; //vão ter no maximo 6 unidades aliadas

    Unit[] enemyUnits = new Unit[MAX_UNIT_SIZE]; //vao ter no maximo 6 unidades inimigas

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
        //funções que lidam com estados que são utilizados no Update devem ser Coroutines para evitar bugs,
        //mais para frente fazer uma avaliação do codigo para decidir o que deve ser coroutine ou nao.
        if(state == BattleState.TARGETSELECTIONTURN){//deals with the inputs during TargetSelectionTurn, 
            if (Input.GetButtonDown("Submit"))//isso aqui ta insta sendo ativado quando entra no TARGETSELECTIONTURN
            {
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, new List<Unit> {enemyUnits[SelectedEnemy]});
                enemyUnits[SelectedEnemy].HUD.is_Selected(false);
                StartCoroutine(SelectNextPartyMember());
            }
            else if (Input.GetButtonDown("Direita"))
            {
                StartCoroutine(SelectNextEnemy());
            }
            else if (Input.GetButtonDown("Esquerda"))
            {
                StartCoroutine(SelectPreviousEnemy());
            }
        }
        
    }

    IEnumerator SetupBattle(){
        GameObject partyGO = Instantiate(partyPrefab);
        for(int i = 0; i < 6; i++){
            if (partyGO.transform.GetChild(i).GetComponent<Unit>().unitName == "")//por enquanto estou considerando que se a unidade nao tiver nome, ela nao existe
                continue;
            partyMembers[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            partyMembers[i].HUD = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            partyMembers[i].HUD.SetHUD(partyMembers[i]);
        }

        //Temporario
        GameObject[] enemyGO = new GameObject[enemy_number]; 
        for (int i = 0; i < enemy_number; i++)
        {
            enemyGO[i] = Instantiate(enemyPrefab, enemyBattleStation);
            Transform p = enemyGO[i].transform;
            float x_position = (i - enemy_number/2)*3;//arrumar isso porque isso soh funciona para numeros impares de inimigos, mas tanto faz agora
            p.position = new Vector3(x_position, p.position.y, p.position.z);
            enemyUnits[i] = enemyGO[i].GetComponent<Unit>();
            enemyUnits[i].HUD.SetHUD(enemyUnits[i]);
        }
        

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
            if(!unit || unit.isDead){
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
        SortLists();
        //Play every unit action in order of priority and speed
        foreach (Unit unit in MaxPriorityBattleOrder)//First play all MaxPriority moves in order of user speed
        {
            if(!unit.isDead)
                unit.Move.PerformAction();
        }
        MaxPriorityBattleOrder.Clear();
        
        foreach (Unit unit in PriorityBattleOrder)//Second play all Priority moves in order of user speed
        {
            if(!unit.isDead)
                unit.Move.PerformAction();
        }
        PriorityBattleOrder.Clear();

        foreach (Unit unit in BattleOrder)//Third play all normal Priority moves in order of user speed
        {
            if(!unit.isDead)
                unit.Move.PerformAction();
        }
        BattleOrder.Clear();

        foreach (Unit unit in LowPriorityBattleOrder)//Last play all low Priority moves in order of user speed
        {
            if(!unit.isDead)
                unit.Move.PerformAction();
        }
        LowPriorityBattleOrder.Clear();

        //reset every unit guard
        foreach (Unit unit in partyMembers)
        {
            if(unit)
                unit.SetGuard(false);
        }
        foreach (Unit unit in enemyUnits)
        {
            if(unit)
                unit.SetGuard(false);
        }

        //End Battle Turn, return to moveselectionturn
        if(!(state == BattleState.WON || state == BattleState.LOST)){
            MoveSelectionTurn();
        }
        
    }

    IEnumerator TargetSelectionTurn(){//talvez se deva passar algum parametro aqui que indique se a ação acerta mais de 1 alvo
        yield return null;//waits 1 frame
        state = BattleState.TARGETSELECTIONTURN;
        foreach (Transform child in ActionMenu.transform)//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);

        //Loop to get an alive unit
        while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            SelectedEnemy++;
            if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy = 0;
        }
        enemyUnits[SelectedEnemy].HUD.is_Selected(true);
    }

    IEnumerator SelectFirstPartyMember(){
        state = BattleState.MOVESELECTIONTURN;
        while ((!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead)
                && SelectedPartyMember < 6){
            SelectedPartyMember++;
        }
        if (SelectedPartyMember >= 6){
            print("alguma coisa deu errado");
        }
        else{
            partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    //seleciona proximo membro da equipe para seleção de Ação
    IEnumerator SelectNextPartyMember(){
        state = BattleState.MOVESELECTIONTURN;
        partyMembers[SelectedPartyMember].HUD.is_Selected(false); 
        SelectedPartyMember++;
        while (SelectedPartyMember < 6 && 
              (!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead)){
            SelectedPartyMember++;
        }
        if (SelectedPartyMember >= 6){
            SelectedPartyMember = 0;
            print("Entrando no BattleFase\n");
            StartCoroutine(BattleTurn());
        }
        else{
            partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    //Seleciona o Membro anterior para refazer sua ação
    IEnumerator SelectPreviousPartyMember(){//isso soh da pra testar quando fizer a logica de cancelar ações
        state = BattleState.MOVESELECTIONTURN;
        partyMembers[SelectedPartyMember].HUD.is_Selected(false); 
        int Atual = SelectedPartyMember;
        SelectedPartyMember--;
        while (SelectedPartyMember >= 0 &&
            (!partyMembers[SelectedPartyMember] || partyMembers[SelectedPartyMember].isDead))
        {
            SelectedPartyMember--;
        }
        if (SelectedPartyMember < 0){
            SelectedPartyMember = Atual;
        }
        else{
            partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
            ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    IEnumerator SelectNextEnemy(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        enemyUnits[SelectedEnemy].HUD.is_Selected(false); 
        SelectedEnemy++;
        if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy = 0;
        //Loop to get an alive unit, assumes that there is at least 1 alive unit
        while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            SelectedEnemy++;
            if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy = 0;
        }
        enemyUnits[SelectedEnemy].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
    }


    IEnumerator SelectPreviousEnemy(){// to do
        enemyUnits[SelectedEnemy].HUD.is_Selected(false); 
        SelectedEnemy--;
        if(SelectedEnemy < 0)
                SelectedEnemy = enemyUnits.Length - 1;
        //Loop to get an alive unit
        while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            SelectedEnemy--;
            if(SelectedEnemy < 0)
                SelectedEnemy = enemyUnits.Length - 1;
        }
        enemyUnits[SelectedEnemy].HUD.is_Selected(true); 
        yield return null;//waits 1 frame
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

            case 2:{//Escape TO DO
                //registra ataque e alvo e depois seleciona proximo membro para registrar o movimento
                StartCoroutine(SelectNextPartyMember());
            }
            break;

            default:
            break;
        }
    }

    //Ordena as listas de ordem de batalha por ordem descrescente de velocidade
    void SortLists(){
        Comparer comparador = new Comparer();
          
        LowPriorityBattleOrder.Sort(comparador);
        BattleOrder.Sort(comparador);
        PriorityBattleOrder.Sort(comparador);
        MaxPriorityBattleOrder.Sort(comparador);
    }

    public void TestBattleEnd(){
        print("Testando final da batalha\n");
        for(int i = 0; i < MAX_UNIT_SIZE; i++){//checa se tem inimigos vivos
            if (enemyUnits[i] && !enemyUnits[i].isDead)//se tiver pelo menos 1 vivo, para de checar
                break;
            if(i == MAX_UNIT_SIZE - 1)
                StartCoroutine(Won());
        }
        for(int i = 0; i < MAX_UNIT_SIZE; i++){//checa se tem aliados vivos
            if (partyMembers[i] && !partyMembers[i].isDead)//se tiver pelo menos 1 vivo, para de checar
                break;
            if(i == MAX_UNIT_SIZE - 1)
                StartCoroutine(Lost());
        }
    }

    IEnumerator Won(){
        print("YOU WIN");
        dialogueText.text = "You WIN!";
        state = BattleState.WON;
        //PlayFanfare();
        yield return new WaitForSeconds(7f);
        //ResultScreen();
    }

    IEnumerator Lost(){
        dialogueText.text = "You LOSE!";
        state = BattleState.LOST;
        //PlayLoseTheme();
        yield return new WaitForSeconds(7f);
        //GameOverScreen();
    }
}
