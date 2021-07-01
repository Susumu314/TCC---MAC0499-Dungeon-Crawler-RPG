using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

/**
* Comparador para ordenação das listas de ordem de batalha baseada na velocidade das unidades,
* as listas são ordenadas por ordem decrescente de velocidade.
*/
class Comparer : IComparer<Unit>
{
    /**
    * Compara a velocidade entre a unidade x e a y
    * 
    * @retval NEGATIVO y.speed é menor que x.speed.
    * @retval ZERO y.speed é igual a x.speed.
    * @retval POSITIVO y.speed é maior que x.speed.
    *
    * @param x Uma das unidades a ser comparada
    * @param y Uma das unidades a ser comparada
    */
    public int Compare(Unit x, Unit y)
    {
        if (!x || !y)
        {
            return 0;
        }
          
        // CompareTo() method
        return (y.speed*y.speedModifier).CompareTo(x.speed*x.speedModifier);
          
    }
}

public enum BattleState { START, MOVESELECTIONTURN, TARGETSELECTIONTURN, BATTLETURN, WON, LOST }

/**
* Classe responsável pelo gerenciamento das batalhas do jogo.
*/
public class Battle_System : MonoBehaviour
{
    private int enemy_number = 6; //numero de inimigos temporario ate decidir direito como vão ser os grupos de inimig
                                  //vai ser aleatorio o numero de inimigos e os tipos de inimigos?
                                  //vão ser pré definidos grupos de inimigos por local igual etrian odyssey?
    private const int BAGSIZE = 60;//tamanho maximo da mochila 
    private const int MAX_UNIT_SIZE = 6;

    //Indica qual unidade está selecionada para seleção de ação
    int SelectedPartyMember = 0;

    int SelectedEnemy = 0;

    int TargetPartyMember = 0;

    public BattleState state;
    public GameObject enemyPrefab;
    public Transform enemyBattleStation;

    public Text dialogueText;

    public GameObject partyHUD;

    Unit[] partyMembers = new Unit[MAX_UNIT_SIZE]; //vão ter no maximo 6 unidades aliadas, de 0 a 2 é frontrow, de 3 a 5 é backrow

    Unit[] enemyUnits = new Unit[MAX_UNIT_SIZE]; //vao ter no maximo 6 unidades inimigas, de 0 a 2 é frontrow, de 3 a 5 é backrow

    public GameObject ActionMenu;
    private ScrollMenuContent bagMenuContent;

    private List<Unit> MaxPriorityBattleOrder = new List<Unit>();//nessa lista serão guardados unidades cuja ação é de prioridade maxima,
                                             // por enquanto só Guard tem esse tipo de prioridade já que é uma ação que deve ocorrer antes de qualquer outra
    private List<Unit> PriorityBattleOrder = new List<Unit>(); //Nessa lista vai ficar todos os personagens que usarem habilidades com prioridade

    private List<Unit> BattleOrder = new List<Unit>(); //Nessa lista serão colocados todas as unidades por ordem de velocidade,
                                   //de ser feito um sorting por ordem de velocidade antes de ser passado para
                                   //a função que executa as ações no turno de batalha
    private List<Unit> LowPriorityBattleOrder = new List<Unit>(); //Lista para unidade que executam movimentos de baixa prioridade no seu turno 

    private bool VerticalPressed = false;
    private bool DirectionalPressed = false;
    private GameObject[] enemyGO;

    public Bag bag;

    public int expEarned = 0;

    private Skill.TARGET_TYPE targetMode = Skill.TARGET_TYPE.SINGLE;

    private bool Battle_SystemReferenceOnSkillMenu = false;
    /**
    * Chamado no primeiro frame em que o objeto é instanciado
    */
    void Start()
    {
        bagMenuContent = ActionMenu.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<ScrollMenuContent>();
        ActionMenu.transform.GetChild(0).GetComponent<ActionMenu>().Battle_System = this;
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        switch (GameManager.Instance.battleID)
        {   
            case GameManager.BattleID.RandomBattle1://toca a intro só as vezes
                if(Random.Range(0,2) == 0)
                    AudioManager.instance.Play("Battle1_intro");
                else
                    AudioManager.instance.Play("Battle1_loop");
            break;
            default:
            break;
        }
    }

    /**
    * No Update é checado os inputs do usuário
    */
    public void Update()
    {
        TargetSelection();
        CancelAction();
    }
    
    /**
    * Cancela a ultima ação feita e volta para o estado anterior
    * Se cancelada ação durante escolha de alvo, volta para seleção de ação da unidade atualmente selecionada,
    * caso cancelada ação durante a seleção de ação da unidade atualmente selecionada, volta à seleção de
    * ação da unidade anterior.
    */

    private void CancelAction(){
        if(Input.GetButtonDown("Cancel")){
            if(state == BattleState.MOVESELECTIONTURN){
                if(ActionMenu.transform.GetChild(0).gameObject.activeInHierarchy){//só volta para o personagem anterior se estiver no main menu do menu de ações
                    StartCoroutine(SelectPreviousPartyMember());
                }
            }
            else if(state == BattleState.TARGETSELECTIONTURN){
                for (int i = 0; i < 6; i++){
                    if(enemyUnits[i] && enemyUnits[i].HUD.isSelected){
                        enemyUnits[i].HUD.is_Selected(false);
                    }
                    if(partyMembers[i] && partyMembers[i].HUD.isTarget){
                        partyMembers[i].HUD.is_Target(false);
                    }
                }
                RemoveAction();
                state = BattleState.MOVESELECTIONTURN;
                ActionMenu.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    

    private void TargetSelection(){
        if(state == BattleState.TARGETSELECTIONTURN){//deals with the inputs during TargetSelectionTurn,
            if (targetMode == Skill.TARGET_TYPE.SINGLE){ 
                if (Input.GetButtonDown("Submit"))
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
                else if(Input.GetAxisRaw("Vertical") != 0)
                {
                    if(!VerticalPressed)
                    {
                        // Call your event function here.
                        VerticalPressed = true;
                        StartCoroutine(SelectNextRowEnemy());
                    }
                }
                else if( Input.GetAxisRaw("Vertical") == 0)
                {
                    VerticalPressed = false;
                } 
            }

            else if(targetMode == Skill.TARGET_TYPE.ROW){
               if (Input.GetButtonDown("Submit"))//isso aqui ta insta sendo ativado quando entra no TARGETSELECTIONTURN
                {
                    List<Unit> targetList = new List<Unit>();
                    for (int i = 0; i < 6; i++)
                    {
                        if(enemyUnits[i] && enemyUnits[i].HUD.isSelected){
                            targetList.Add(enemyUnits[i]);
                            enemyUnits[i].HUD.is_Selected(false);
                        }
                    }
                    partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, targetList);
                    StartCoroutine(SelectNextPartyMember());
                }
                else if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
                {
                    if(!DirectionalPressed)
                    {
                        DirectionalPressed = true;
                        StartCoroutine(SelectNextRow());
                    }
                }
                else if( Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
                {
                    DirectionalPressed = false;
                }
            }

            else if(targetMode == Skill.TARGET_TYPE.SINGLE_ALLY){
                if (Input.GetButtonDown("Submit"))
                {
                    partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.NULL, new List<Unit> {partyMembers[TargetPartyMember]});
                    partyMembers[TargetPartyMember].HUD.is_Target(false);
                    StartCoroutine(SelectNextPartyMember());
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
                    StartCoroutine(SelectNextPartyMember());
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
                    StartCoroutine(SelectNextPartyMember());
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


    /**
    * @brief Faz o setup inicial das scenes de batalha.
    * Lê as informações dos membros da equipe do jogador 
    * e dos monstros do grupo especificado pela a Table de Random Encounters,
    * e os instanciam na cena de batalha.
    * Depois avança para a etapa de seleção de movimentos do jogador.
    */
    IEnumerator SetupBattle(){
        //fazer essa instanciação do player por algum arquivo json
        GameObject partyGO = GameManager.Instance.party;
        bag = GameManager.Instance.party.GetComponent<Bag>();
        for(int i = 0; i < 6; i++){
            if (partyGO.transform.GetChild(i).GetComponent<Unit>().species == "")//por enquanto estou considerando que se a unidade nao tiver nome, ela nao existe
                continue;
            partyMembers[i] = partyGO.transform.GetChild(i).GetComponent<Unit>();
            partyMembers[i].InitStats();
            partyMembers[i].ResetStatMods(); 
            partyMembers[i].BattleSystemReference(this);
            partyMembers[i].isBackLine = i/3;
            partyMembers[i].HUD = partyHUD.transform.GetChild(i).GetComponent<PlayerBattleHUD>();
            partyMembers[i].HUD.SetHUD(partyMembers[i]);
        }

        InstantiateEnemyUnits();
        

        dialogueText.text = "A wild demon horde has appeared!";

        bagMenuContent.InitBagMenu(bag, this);

        yield return new WaitForSeconds(2f);

        MoveSelectionTurn();
    }
    //todas funçoes que mudam o estado do state devem ser Coroutines para evitar bugs
    
    /**
    * Função que inicia o Turno de Seleção de movimentos durante a batalha.
    * Primeiro seleciona os movimentos dos inimigos, e então chama a rotina para seleção
    * de movimento do primeiro personagem do grupo do jogador.
    */
    void MoveSelectionTurn(){
        //At the start of the MOveSelectionTurn, input the AI moves
        //talvez fazer uma AI que trapaceia igual nos primeiros pokemons, onde o input do computador era
        //dado depois que o jogador escolhia a ação
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
    
    /**
    * @brief Realiza as ações do turno de batalha.
    * Organiza as listas de ordem de batalha de acordo com as prioridas das ações,
    * e executa as ações de cada personagem vivo.
    * Se pelo menos 1 unidade inimiga e 1 unidade do jogador estiverem vivas, voltar ao turno de seleção de ações.
    */
    IEnumerator BattleTurn(){
        state = BattleState.BATTLETURN;
        //sort BattleOrder lists based on user speed and speedmodifiers
        SortLists();
        //Play every unit action in order of priority and speed
        foreach (Unit unit in MaxPriorityBattleOrder)//First play all MaxPriority moves in order of user speed
        {
            if(!unit.isDead){
                unit.HUD.is_Selected(true);//isso aqui funciona bem para as unidades do jogador, mas nao tao bem para as unidade inimigas, criar uma nova função semelhante para funcionar para os 2 (por enquanto é o sufiente)
                yield return unit.Move.PerformAction();
                unit.HUD.is_Selected(false);
            }
        }
        MaxPriorityBattleOrder.Clear();
        
        foreach (Unit unit in PriorityBattleOrder)//Second play all Priority moves in order of user speed
        {
            if(!unit.isDead){
                unit.HUD.is_Selected(true);//isso aqui funciona bem para as unidades do jogador, mas nao tao bem para as unidade inimigas, criar uma nova função semelhante para funcionar para os 2 (por enquanto é o sufiente)
                yield return unit.Move.PerformAction();
                unit.HUD.is_Selected(false);
            }
        }
        PriorityBattleOrder.Clear();

        foreach (Unit unit in BattleOrder)//Third play all normal Priority moves in order of user speed
        {
            if(!unit.isDead){
                unit.HUD.is_Selected(true);//isso aqui funciona bem para as unidades do jogador, mas nao tao bem para as unidade inimigas, criar uma nova função semelhante para funcionar para os 2 (por enquanto é o sufiente)
                yield return unit.Move.PerformAction();
                unit.HUD.is_Selected(false);
            }
        }
        BattleOrder.Clear();

        foreach (Unit unit in LowPriorityBattleOrder)//Last play all low Priority moves in order of user speed
        {
            if(!unit.isDead){
                unit.HUD.is_Selected(true);//isso aqui funciona bem para as unidades do jogador, mas nao tao bem para as unidade inimigas, criar uma nova função semelhante para funcionar para os 2 (por enquanto é o sufiente)
                yield return unit.Move.PerformAction();
                unit.HUD.is_Selected(false);
            }
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

    /**
    * @brief Etapa de seleção de alvo para habilidades e ataques.
    * Após o jogador selecionar uma ações, o turno de seleção de alvo permite selecionar quais 
    * unidades serão o alvo da ação
    */
    IEnumerator TargetSelectionTurn(){
        yield return null;
        dialogueText.text = "Select Target";
        state = BattleState.TARGETSELECTIONTURN;
        // foreach (Transform child in ActionMenu.transform)//esconde o menu principal para escolher o alvo
        //     child.gameObject.SetActive(false);

        //Loop to get an alive unit
        if (targetMode == Skill.TARGET_TYPE.SINGLE){
            while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
                SelectedEnemy++;
                if(SelectedEnemy >= enemyUnits.Length)
                    SelectedEnemy = 0;
            }
            enemyUnits[SelectedEnemy].HUD.is_Selected(true);
        }

        else if (targetMode == Skill.TARGET_TYPE.ROW){
            while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
                SelectedEnemy++;
                if(SelectedEnemy >= enemyUnits.Length)
                    SelectedEnemy = 0;
            }
            if(SelectedEnemy <  3){
                for (int i = 0; i < 3; i++)
                {
                    if(enemyUnits[i] && !enemyUnits[i].isDead){
                        enemyUnits[i].HUD.is_Selected(true);
                    }
                }
            }
            else{
                for (int i = 3; i < 6; i++)
                {
                    if(enemyUnits[i] && !enemyUnits[i].isDead){
                        enemyUnits[i].HUD.is_Selected(true);
                    }
                }
            }
        }

        else if (targetMode == Skill.TARGET_TYPE.SINGLE_ALLY){
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
            OpenMenu();//Abre o menu de ações
        }
        state = BattleState.MOVESELECTIONTURN;
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o proximo membro da equipe do jogador que está vivo para seleção de ação.
    */
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
            OpenMenu();//Abre o menu de ações
        }
        yield return null;//waits 1 frame
    }

    /**
    * Seleciona o membro anterior da equipe do jogador que está vivo para seleção de ação.
    */
    IEnumerator SelectPreviousPartyMember(){//precisa apagar a unidade atual da lista de ações
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
        partyMembers[SelectedPartyMember].HUD.is_Selected(true); 
        RemoveAction();
        OpenMenu();//Abre o menu de ações
        yield return null;//waits 1 frame
    }

    /**
    * Remove a ação da unidade selecionada atualmente das listas de ações
    */
    private void RemoveAction(){
        bool removeu = false;
        removeu = BattleOrder.Remove(partyMembers[SelectedPartyMember]);
        if(!removeu){
            removeu = PriorityBattleOrder.Remove(partyMembers[SelectedPartyMember]);
        }
        if(!removeu){
            removeu = LowPriorityBattleOrder.Remove(partyMembers[SelectedPartyMember]);
        }
        if(!removeu){
            MaxPriorityBattleOrder.Remove(partyMembers[SelectedPartyMember]);
        }
    }


    /**
    * Seleciona o proximo inimigo que está vivo para seleção de alvo.
    */
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

    /**
    * Seleciona o proximo inimigo que está vivo para seleção de alvo.
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
    IEnumerator SelectNextRowEnemy(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (SelectedEnemy > 2){
            frontRow = false; 
        }
        enemyUnits[SelectedEnemy].HUD.is_Selected(false); 
        SelectedEnemy += 3;
        if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            if (frontRow)
                SelectedEnemy = 3;
            else
                SelectedEnemy = 0;
        }
        while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            SelectedEnemy++;
            if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy -= MAX_UNIT_SIZE;
        }
        enemyUnits[SelectedEnemy].HUD.is_Selected(true); 
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
    * Seleciona o proximo linha de inimigos como alvos
    */
    IEnumerator SelectNextRow(){//isso aqui eh um IEnumerator para que o player nao consigo fazer mais do que uma ação dessas por frame
        bool frontRow = true; 
        if (SelectedEnemy > 2){
            frontRow = false; 
            for (int i = 3; i < 6; i++)
            {
                if(enemyUnits[i] && !enemyUnits[i].isDead){
                    enemyUnits[i].HUD.is_Selected(false);
                }
            }
        }
        else{
            for (int i = 0; i < 3; i++)
            {
                if(enemyUnits[i] && !enemyUnits[i].isDead){
                    enemyUnits[i].HUD.is_Selected(false);
                }
            }
        }

        SelectedEnemy += 3;
        if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy -= MAX_UNIT_SIZE;
        //Loop para achar uma unidade viva caso a pretendida inicialmente esteja morta ou nao exista, procura primeiro na coluna diferente, caso não ache, procura na coluna atual
        if(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            if (frontRow)
                SelectedEnemy = 3;
            else
                SelectedEnemy = 0;
        }
        while(!enemyUnits[SelectedEnemy] || enemyUnits[SelectedEnemy].isDead){
            SelectedEnemy++;
            if(SelectedEnemy >= enemyUnits.Length)
                SelectedEnemy -= MAX_UNIT_SIZE;
        }
        if(SelectedEnemy <  3){
            for (int i = 0; i < 3; i++)
            {
                if(enemyUnits[i] && !enemyUnits[i].isDead){
                    enemyUnits[i].HUD.is_Selected(true);
                }
            }
        }
        else{
            for (int i = 3; i < 6; i++)
            {
                if(enemyUnits[i] && !enemyUnits[i].isDead){
                    enemyUnits[i].HUD.is_Selected(true);
                }
            }
        }
        
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
    * Seleciona o inimigo anterior que está vivo para seleção de alvo.
    */
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
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnActionButton(int index)  //Como seria para cancelar essa ação?
    {
        if (state != BattleState.MOVESELECTIONTURN){
            return;
        }
        foreach (Transform child in ActionMenu.transform)//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        switch (index)
        {
            case 0:{//Attack
                //registra ataque e alvo
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.ATTACK);//registra a ação
                BattleOrder.Add(partyMembers[SelectedPartyMember]);// a ação de atacar é uma ação de prioridade normal
                targetMode = Skill.TARGET_TYPE.SINGLE;
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
            case 3:{//Ação de captura
                //registra captura e alvo
                partyMembers[SelectedPartyMember].Move.SetAction(BattleAction.Act.CAPTURE);//registra a ação
                MaxPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);// a ação de capturar é de prioridade maxima
                targetMode = Skill.TARGET_TYPE.SINGLE;
                StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
            }
            break;
            default:
            break;
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
        if (state != BattleState.MOVESELECTIONTURN){
            return;
        }
        foreach (Transform child in ActionMenu.transform){//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        }

        int[] a = partyMembers[SelectedPartyMember].Move.SetSkill(index);//registra a ação
        switch (a[0])
        {
            case 0:
                LowPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 1:
                BattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 2:
                PriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 3:
                MaxPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            default:
            break;
        }
        targetMode = (Skill.TARGET_TYPE)a[1];
        StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
        
    }

    /**
    *Ordena as listas de ordem de batalha por ordem descrescente de velocidade.
    */
    void SortLists(){
        Comparer comparador = new Comparer();
          
        LowPriorityBattleOrder.Sort(comparador);
        BattleOrder.Sort(comparador);
        PriorityBattleOrder.Sort(comparador);
        MaxPriorityBattleOrder.Sort(comparador);
    }

    /**
    * Verifica se a batalha terminou.
    */
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

    /**
    * Função chamada quando o jogador ganha a batalha,
    * ao ser chamada, realiza a rotina de vitória.
    */
    IEnumerator Won(){
        print("YOU WIN");
        dialogueText.text = "You WIN!";
        state = BattleState.WON;
        //PlayFanfare();
        yield return new WaitForSeconds(1f);
        //Calcula o numero de unidades do jogador vivos:
        int s = 0;
        foreach (Unit unit in partyMembers)
        {   
            if(unit){
                if(!unit.isDead){
                    s++;
                }
            }
        }
        Debug.Log(s);
        //calcula a experiencia que cada unidade deve ganhar
        foreach (Unit unit in partyMembers)
        {
            if(unit){
                if(!unit.isDead){
                    yield return unit.GainExp(Mathf.RoundToInt((float)expEarned/(float)s));
                }
            }
        }
        GameManager.Instance.state = GameManager.State.Overworld;
        SceneManager.LoadScene(GameManager.Instance.CurrentOverworldScene);
        //ResultScreen();
    }

    /**
    * Função chamada quando o jogador perde a batalha,
    * ao ser chamada, realiza a rotina de derrota.
    */
    IEnumerator Lost(){
        dialogueText.text = "You LOSE!";
        state = BattleState.LOST;
        //PlayLoseTheme();
        yield return new WaitForSeconds(2f);
        //GameOverScreen();
        GameManager.Instance.state = GameManager.State.Overworld;
        SceneManager.LoadScene(GameManager.Instance.CurrentOverworldScene);
    }

    /**
    * Cria e instância o grupo de inimigos gerado pelo RandomEncounters.cs
    */

    private void InstantiateEnemyUnits(){
        enemyGO = new GameObject[enemy_number]; 
        List<string> enemy_formation = GameManager.Instance.encounter.enemy_formation;
        int maxLvl =  GameManager.Instance.encounter.max_level;
        int minLvl = GameManager.Instance.encounter.min_level;
        for (int i = 0; i < enemy_number; i++)
        {
            if(enemy_formation[i] == ""){
                continue;
            }
            enemyGO[i] = Instantiate(enemyPrefab, new Vector3(enemyBattleStation.transform.position.x + 3.5f*(1 - 0.25f*Mathf.Floor((i/3)))*(i - (1 + 3*Mathf.Floor((i/3)))), enemyBattleStation.transform.position.y + Mathf.Floor((i/3)), enemyBattleStation.transform.position.z + 2*Mathf.Floor((i/3))+1), Quaternion.identity);
            if (i>2){
                enemyGO[i].transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            }
            enemyUnits[i] = enemyGO[i].GetComponent<Unit>();
            enemyUnits[i].InitEnemyUnit(enemy_formation[i], Random.Range(minLvl, maxLvl+1), i/3);
            Debug.Log(i/3);
            enemyUnits[i].InitStats();
            enemyUnits[i].ResetStatMods();
            enemyUnits[i].BattleSystemReference(this);
            enemyUnits[i].HUD.SetHUD(enemyUnits[i]);
        }
    }
    public void OpenMenu(){
        ActionMenu.transform.GetChild(0).gameObject.SetActive(true);//abre main menu
        GameObject skillMenu = ActionMenu.transform.GetChild(1).gameObject;
        for (int i = 0; i < 4; i++)
        {   
            int skillID = partyMembers[SelectedPartyMember].skillList[i];
            if(skillID == -1){
                skillMenu.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "Empty";
                continue;
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
            if(!Battle_SystemReferenceOnSkillMenu){
                selectable.Battle_System = this;
            }
        }
        
        bagMenuContent.UpdateButtons(bag);
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnItemButton(int index)  //Como seria para cancelar essa ação?
    {
        if (state != BattleState.MOVESELECTIONTURN){
            return;
        }
        foreach (Transform child in ActionMenu.transform){//esconde o menu principal para escolher o alvo
            child.gameObject.SetActive(false);
        }

        int[] a = partyMembers[SelectedPartyMember].Move.SetItem(index);//registra a ação
        switch (a[0])
        {
            case 0:
                LowPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 1:
                BattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 2:
                PriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            case 3:
                MaxPriorityBattleOrder.Add(partyMembers[SelectedPartyMember]);
            break;
            default:
            break;
        }
        targetMode = (Skill.TARGET_TYPE)a[1];
        StartCoroutine(TargetSelectionTurn());//inicia a ação de escolher um alvo
        
    }


}
