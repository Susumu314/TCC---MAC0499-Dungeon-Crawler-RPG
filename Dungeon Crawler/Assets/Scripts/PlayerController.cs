using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 16f;
    public float step = 4f;
    public float rotationSpeed = 360f;
    public Transform movePoint;
    public Vector3 Target;

    private List<Input> inputBuffer = new List<Input>();

    public LayerMask CollisionMask;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
        CollisionMask = (1<<8);//wall
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, movePoint.rotation, rotationSpeed * Time.deltaTime);
        
        //modificar isso aqui para funcionar com buffer de input para o movimento ser mais fluido
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f*step && Quaternion.Angle(transform.rotation, movePoint.rotation) <= 0.1f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                movePoint.Rotate(0f, 90f * Input.GetAxisRaw("Horizontal"), 0f);
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                Target = movePoint.position + transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                if(!Physics.Linecast(transform.position, Target , CollisionMask))//check if there is a wall
                {
                    movePoint.position += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
                }
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Strafe")) == 1f){
                Target = movePoint.position + transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                if(!Physics.Linecast(transform.position, Target , CollisionMask))//check if there is a wall
                {
                    movePoint.position += transform.rotation * Vector3.right * Input.GetAxisRaw("Strafe")*step;
                }
            }
        }
    }
}
