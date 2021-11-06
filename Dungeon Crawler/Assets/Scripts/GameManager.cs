using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe singleton utilizada para gerenciar informações que devem ser salvas
* ou compartilhadas entre diferentes cenas.
*/

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    private void Awake()
    {
        print("Awake");
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public enum State {
        Start,
        Overworld,
        InRandomEncounter,
        InTutorial,
        Evolution,
        LearnSkill,
        GameOver,
    }

    public enum BattleID{
        RandomBattle1,
        RandomBattle2,
        Rival,
        FOE1,
        GYM_LEADER,
    }

    public BattleID battleID;

    private bool isInit = false;
    public State state;

    public GameObject party;//referencia para a equipe do jogador
    public Vector3 overworldPlayerPosition = new Vector3(); // por enquanto essa é a unica variavel importante de se guardar quando se sai do overworld para uma battle_scene
    public Quaternion overworldPlayerRotation = new Quaternion();
    public int CurrentOverworldScene = 0;
    public int CurrentOverworldID = 0;
    public bool InTutorial = false;
    public RandomEncouters.encounter encounter = new RandomEncouters.encounter(1, 1, new List<string>() {"", "", "", "", "", ""});
    public string eventFlag = "Batalha1";
    private string lastTutorial;
    public class LootAcquired
    {
        public int OverworldID;
        public int ID;

        public LootAcquired(int OverworldID, int ID){
            this.OverworldID = OverworldID;
            this.ID = ID;
        }
    }
    //Guarda o ID de cada item coletado no overworld
    public List<LootAcquired> loot = new List<LootAcquired>();

    public void AddLoot(int ID){
        loot.Add(new LootAcquired(CurrentOverworldID, ID));
    }

    public class InteractableUsed
    {
        public int OverworldID;
        public int ID;

        public InteractableUsed(int OverworldID, int ID){
            this.OverworldID = OverworldID;
            this.ID = ID;
        }
    }
    //Guarda o ID de cada objeto interativel no overworld
    public List<InteractableUsed> interactables = new List<InteractableUsed>();

    public void AddInteractable(int ID){
        interactables.Add(new InteractableUsed(CurrentOverworldID, ID));
    }

    public class Tutorial
    {
        public string tutorialName;
        public bool cleared;

        public Tutorial(string tutorialName, bool cleared = false){
            this.tutorialName = tutorialName;
            this.cleared = cleared;
        }
    }

    public List<Tutorial> tutorials = new List<Tutorial>();

    public enum Event {GotFirstDemon, EnteredForest, EventsSize}
    public List<bool> EventList = new List<bool>();

    public bool GameOver = false;

    /**
    * Abre um tutorial 
    */
    public void StartTutorial(string name){
        Instantiate(Resources.Load("Tutoriais/" + name), GameObject.Find("Canvas").transform);
        InTutorial = true;
        lastTutorial = name;
    }

    /**
    * Fecha Tutorial
    */
    public void CloseTutorial(){
        Tutorial t;
        Debug.Log(lastTutorial);
        for (int i = 0; i < tutorials.Count; i++)
        {
            if(tutorials[i].tutorialName == lastTutorial){
                t = tutorials[i];
                t.cleared = true;
                InTutorial = false;
                i = tutorials.Count;
            }
        }
    }

    /**
    * Inicialização de alguns parametros.
    */
    public void Init() {
        if (isInit) return;
        Debug.Log("Init();");
        isInit = true;
        state = State.Start;
        party = gameObject.transform.GetChild(0).gameObject;
        //adiciona os tutoriais a lista de tutoriais a fazer
        tutorials.Add(new Tutorial("Batalha1"));
        tutorials.Add(new Tutorial("Batalha2"));
        tutorials.Add(new Tutorial("Inputs"));
        tutorials.Add(new Tutorial("SpecialSpots"));
        tutorials.Add(new Tutorial("Switch"));
        tutorials.Add(new Tutorial("Taming"));
        //inicia a lista de eventos, todos false
        for(int i = 0; i < (int)Event.EventsSize; i++)
        {
            EventList.Add(false);
        }
    }

    /**
    * Função para checar se o GameManager já foi iniciado.
    *
    * @retval TRUE Os parâmetros de inicio de jogo foram iniciados.
    * @retval FALSE Os parâmetros de inicio de jogo NÃO foram iniciados.
    */
    public bool GetIsInit(){
        return isInit;
    }

    /**
    * Função que permite editar e salvar informações da equipe do jogador toda vez que a equipe sofrer alguma mudança.
    * 
    * @param party Array contendo todas as informações da equipe do jogador. 
    */
    // public void EditParty(Unit[] party){//não tenho ctz se essa merda funciona
    //     partyMembers = party;
    // }

    /**
    * Função responsável por armazenar a informação do grupo de inimigos que serão instanciados durante uma batalha
    *
    * @param um objeto encounter contendo o nivel minimo, nivel maximo, e nomes dos inimigos a serem instanciados na batalha.
    */
    public void SaveEncounter(RandomEncouters.encounter e){
        encounter = e;
        // foreach (string name in encounter.enemy_formation) //debugation baby
        // {
        //     Debug.Log(name+"\n");
        // }
    }

    /**
    * Salva a posição no Overworld do jogador.
    * 
    * @param PlayerTransform Transform que contém todas as informações de posicionamento do jogador.
    */
    public void SaveOverwoldPlayer(Transform PlayerTransform){
        overworldPlayerPosition = new Vector3(Mathf.Round(PlayerTransform.position.x), PlayerTransform.position.y ,Mathf.Round(PlayerTransform.position.z)); //rounding needs to be done because the encounters happens before a step is completely finish
        overworldPlayerRotation = PlayerTransform.rotation;
    }

    /**
    * Salva o index da cena atual, utilizado para retornar para a cena correta após o fim de uma batalha
    *   
    * @param sceneIndex Index da cena que precisa ser guardada.
    */
    public void SaveCurrentOverworldScene(int sceneIndex){
        CurrentOverworldScene = sceneIndex;
    }

    /**
    *
    */
    public void CapturedUnit(Unit u){
        /*Procura um espaço vazio no grupo para alocar o unidade capturada*/
        bool achou = false;
        int i = 0;
        Unit unit;
        unit = party.transform.GetChild(i).GetComponent<Unit>();
        while(!achou && i < 6){
            unit = party.transform.GetChild(i).GetComponent<Unit>();
            if(unit.species == ""){
                achou = true;
                continue;
            }
            i++;
        }
        if (achou){
            unit.species = u.species;
            unit.unitName = u.species;
            unit.unitLevel = u.unitLevel;
            unit.InitStats();
            unit.currentHP = u.currentHP;
            unit.isPlayerUnit = true;
        }
        else{
            //aloca o demonio no Demonomicon
        }
    }

}