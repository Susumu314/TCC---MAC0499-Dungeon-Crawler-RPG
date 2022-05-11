using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    void Update()
    {
        Next();
    }

    private void Next(){
        if(Input.GetButtonDown("Submit")){
            GameManager.Instance.CloseTutorial();
            Destroy(this.gameObject);
        }
    }
}
