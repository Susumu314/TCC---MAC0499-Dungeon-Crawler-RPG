using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Animation : MonoBehaviour
{
    private bool played = false;//garante que um som soh vai ser tocado 1 vez por chamada de animação
    void DestroyGameObject(){
        Destroy(this.gameObject);
    }

    void Play(string SFX){
        if(!played){
            AudioManager.instance.Play(SFX);
            played = true;
        }
    }
}
