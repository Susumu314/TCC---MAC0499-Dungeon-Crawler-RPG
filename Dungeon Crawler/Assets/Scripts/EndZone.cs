using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    public void OpenScreen(){
        Instantiate(Resources.Load("Tutoriais/EndScreen"), GameObject.Find("Canvas").transform);
    }
}
