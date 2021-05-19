using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public float despawnTime;
    private TextMeshPro damageText;
    public float moveSpeed;
    private Color textColor;
    public float despawnSpeed;
    private float randomAngle;
    
    public void Awake(){
        damageText = transform.GetComponent<TextMeshPro>();
        textColor = damageText.color;
    }

    public void Setup(int damage){
        damageText.text = damage.ToString();
        if(damage > 9999999f){
            damageText.text = Mathf.Floor(damage/1000000f).ToString() + "M";
        }
        else if(damage > 99999f){
            damageText.text = Mathf.Floor(damage/1000f).ToString() + "K";
        }
        else{
            damageText.text = Mathf.Floor(damage).ToString();
        }
    }

    public void Update(){
        transform.position += new Vector3(0, 1, 0)*Time.deltaTime;
        despawnTime -= Time.deltaTime;
        if (despawnTime <= 0){
            textColor.a -= despawnSpeed*Time.deltaTime;
            damageText.color = textColor;
            if(textColor.a <= 0){
                Destroy(gameObject);
            }
        }
    }
}
