using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float step = 3f;
    public float rotationSpeed = 360f;
    public Transform movePoint;

    private List<Input> inputBuffer = new List<Input>();

    public LayerMask CollisionMask;
    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, movePoint.rotation, rotationSpeed * Time.deltaTime);
        
        //modificar isso aqui para funcionar com buffer de input
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f*step && Quaternion.Angle(transform.rotation, movePoint.rotation) <= 0.1f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                movePoint.Rotate(0f, 90f * Input.GetAxisRaw("Horizontal"), 0f);
            }
            else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                movePoint.position += transform.rotation * Vector3.forward * Input.GetAxisRaw("Vertical")*step;
            }
            //adicionar strafe
        }
    }
}
