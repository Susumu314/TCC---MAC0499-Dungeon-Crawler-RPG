using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<Input> inputBuffer = new List<Input>();

    public LayerMask WallMask;
    public LayerMask EncounterMask;
    // Start is called before the first frame update
    private RandomEncouters REScript;

    public bool isInAnEncounterZone = false;

    private EncounterZone zone;
    private OW_MenuSystem MenuSystem;
 
    private STATES state;

    /**
    * No primeiro frame em que o script roda, é iniciado algumas váriaveis.
    */

    void Start()
    {
        state = STATES.IDLE;
        movePoint.parent = null;
        WallMask = (1<<8);//wall
        EncounterMask = (1<<9);//EncounterZone
        REScript = gameObject.GetComponent<RandomEncouters>();
        MenuSystem = GameObject.Find("OverworldMenu_System").GetComponent<OW_MenuSystem>();
    }

    /**
    * Checa inputs do usuário todo frame e realiza ações.
    */
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, movePoint.rotation, rotationSpeed * Time.deltaTime);
        
        if(MenuSystem.state == OW_MenuSystem.OW_State.FREEROAMING){//só consegue andar quando no estado freeroaming
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f*step && Quaternion.Angle(transform.rotation, movePoint.rotation) <= 0.1f){
                if (state == STATES.WALKING){
                    //terminou de dar um passo
                    REScript.Increment_Encouter(zone.EncounterRate, zone.ZoneID);
                    state = STATES.IDLE;
                }
                if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                    movePoint.Rotate(0f, 90f * Input.GetAxisRaw("Horizontal"), 0f);
                }
                else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                    Target = movePoint.position + transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                    if(!Physics.Linecast(transform.position, Target , WallMask))//check if there is a wall
                    {
                        movePoint.position += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                        state = STATES.WALKING;
                    }
                }
                else if(Mathf.Abs(Input.GetAxisRaw("Strafe")) == 1f){
                    Target = movePoint.position + transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                    if(!Physics.Linecast(transform.position, Target , WallMask))//check if there is a wall
                    {
                        movePoint.position += transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                        state = STATES.WALKING;
                    }
                }
            }
        }
    }

    /**
    * Função utilizada para verificar colisões com o jogador.
    *
    * @param collider Collider do objeto com quem houve uma colisão.
    */
    private void OnTriggerEnter(Collider collider){
        if ((collider.gameObject.layer == 9)) //por enquanto só estou usando essa função para verificar qual é a EncounterZone que o personagem se encontra
            zone = collider.GetComponent<EncounterZone>();
            isInAnEncounterZone = true;
            print("Entrando em nova zona");
    }

    /**
    * Getter para a zona em que o jogador se encontra atualmente.
    *
    * @retval zone.ZoneID Int que identifica uma zona.
    */
    public string GetZoneID(){
        return zone.ZoneID;
    }
}
