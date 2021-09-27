using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
* Classe utilizada para controlar o jogador no Overworld.
*/
public class PlayerController : MonoBehaviour
{
    enum STATES {WALKING, IDLE};
    public float moveSpeed = 16f;
    public float step = 4f;
    public float rotationSpeed = 360f;
    public Transform movePoint;
    public Vector3 Target;
    private ScrollMenuContent bagMenuContent;
    private List<Input> inputBuffer = new List<Input>();

    public LayerMask WallMask;
    public LayerMask EncounterMask;
    // Start is called before the first frame update
    private RandomEncouters REScript;

    public bool isInAnEncounterZone = false;

    private EncounterZone zone;
    private OW_MenuSystem MenuSystem;
 
    private STATES state;
    private int onSpecialTile;

    private OverworldLoot loot;
    private GameObject party;
    private bool tutorialFlag = false;
    private bool facingInteractable = false;
    int layerMask = 1 << 14;
    RaycastHit hit;
    private Interactable interactable;

    void Awake(){
        MenuSystem = GameObject.Find("OverworldMenu_System").GetComponent<OW_MenuSystem>();
    }

    /**
    * No primeiro frame em que o script roda, é iniciado algumas váriaveis.
    */

    void Start()
    {
        state = STATES.IDLE;
        movePoint.parent = null;
        onSpecialTile = 0;
        facingInteractable = false;
        WallMask = (1<<8);//wall
        EncounterMask = (1<<9);//EncounterZone
        REScript = gameObject.GetComponent<RandomEncouters>();
        party = GameObject.Find("/GameManager/Party");
        bagMenuContent = GameObject.Find("/Canvas/OW_ActionMenu").transform.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<ScrollMenuContent>();
    }

    /**
    * Checa inputs do usuário todo frame e realiza ações.
    */
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, movePoint.rotation, rotationSpeed * Time.deltaTime);
        if(tutorialFlag){
            if(!GameManager.Instance.InTutorial){
                tutorialFlag = false;
                MenuSystem.tutorialFlag = false;
            }
        }
        if(MenuSystem.state == OW_MenuSystem.OW_State.FREEROAMING){//só consegue andar quando no estado freeroaming
            if (!tutorialFlag && Vector3.Distance(transform.position, movePoint.position) <= 0.1f*step && Quaternion.Angle(transform.rotation, movePoint.rotation) <= 0.1f){
                //MenuSystem.DialogueBox.SetActive(false);
                CheckForInteractables();
                if (state == STATES.WALKING){
                    //terminou de dar um passo
                    REScript.Increment_Encouter(zone.EncounterRate, zone.ZoneID);
                    state = STATES.IDLE;
                }
                if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                    movePoint.Rotate(0f, 90f * Input.GetAxisRaw("Horizontal"), 0f);
                    if(onSpecialTile == 0) MenuSystem.DialogueBox.SetActive(false);
                }
                else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                    Target = movePoint.position + transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                    if(!Physics.Linecast(transform.position, Target , WallMask))//check if there is a wall
                    {
                        movePoint.position += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                        state = STATES.WALKING;
                        onSpecialTile = 0;
                        MenuSystem.DialogueBox.SetActive(false);
                    }
                }
                else if(Mathf.Abs(Input.GetAxisRaw("Strafe")) == 1f){
                    Target = movePoint.position + transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                    if(!Physics.Linecast(transform.position, Target , WallMask))//check if there is a wall
                    {
                        movePoint.position += transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                        state = STATES.WALKING;
                        onSpecialTile = 0;
                        MenuSystem.DialogueBox.SetActive(false);
                    }
                }
                if(state == STATES.IDLE && onSpecialTile != 0){
                    if(Input.GetButtonDown("Submit")){
                        GetItem();
                    }
                }
                if(state == STATES.IDLE && facingInteractable){
                    if(Input.GetButtonDown("Submit")){
                        Interact();
                    }
                }
            }
        }
    }

    private void CheckForInteractables()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 4, layerMask))
        {
            if(!facingInteractable){
                interactable = hit.collider.gameObject.GetComponent<Interactable>();
                facingInteractable = true;
                ShowDialog("Press Z");
            }
        }
        else{
            facingInteractable = false;
        }
    }

    /**
    * Função utilizada para verificar colisões com o jogador.
    *
    * @param collider Collider do objeto com quem houve uma colisão.
    */
    private void OnTriggerEnter(Collider collider){
        if(onSpecialTile != 0){
            onSpecialTile = 0;
            MenuSystem.DialogueBox.SetActive(false);
        }
        if ((collider.gameObject.layer == 9)){ //por enquanto só estou usando essa função para verificar qual é a EncounterZone que o personagem se encontra
            zone = collider.GetComponent<EncounterZone>();
            isInAnEncounterZone = true;
            print("Entrando em nova zona");
        }
        if((collider.gameObject.layer == 10)){ // se for item abrir prompt para clicar o botao de confirmar para recolher o item
            loot = collider.GetComponent<OverworldLoot>();
            onSpecialTile = 1;// SpecialTile == 1 representa que o jogador está sobre um item no overworld
            ShowDialog("Press Enter to pickup " + loot.item.amount + "x " + Item.ItemList[loot.item.ID].Name);
            Debug.Log("entrou aqui");
        }
        if((collider.gameObject.layer == 11)){
            Debug.Log("Detectou colisao");
            foreach (Unit unit in party.GetComponentsInChildren<Unit>())
            {
                if(unit.species != ""){
                    unit.FullHeal();
                }
            }
        }
        if((collider.gameObject.layer == 12)){ // se for item abrir prompt para clicar o botao de confirmar para recolher o item
            collider.GetComponent<TutorialZone>().OpenTutorial();
            tutorialFlag = true;
            MenuSystem.tutorialFlag = true;
        }
        if((collider.gameObject.layer == 13)){ 
            collider.GetComponent<EndZone>().OpenScreen();
        }
    }

    /**
    * Getter para a zona em que o jogador se encontra atualmente.
    *
    * @retval zone.ZoneID Int que identifica uma zona.
    */
    public string GetZoneID(){
        return zone.ZoneID;
    }

    /**
    * Recolhe o item que está no chão
    */

    private void GetItem(){
        //adiciona o item na mochila e destroi do overworld, salvar em algum lugar o ID local do loot para ele nao ser carregado quando recarregar a scene
        party.GetComponent<Bag>().AddItem(loot.item.ID, loot.item.amount);
        GameManager.Instance.AddLoot(loot.ID);
        Destroy(loot.gameObject);
        onSpecialTile = 0;
        MenuSystem.DialogueBox.SetActive(false);
        Debug.Log("Pegou o item do chão" + loot);
        bagMenuContent.RefreshBagMenu(party.GetComponent<Bag>());
    }

    /**
    * Ativa o script do interactable
    */

    private void Interact(){
        interactable.Act(this);        
    }

    /**
    * Mostra mensagem na caixa de dialogo
    */
    public void ShowDialog(string text){
        MenuSystem.DialogueBox.SetActive(true);
        MenuSystem.dialogueText.text = text;
    }

}
