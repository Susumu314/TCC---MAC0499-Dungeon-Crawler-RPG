using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
* Função baseada na função ensinada em https://medium.com/nice-things-ios-android-development/basic-2d-screen-shake-in-unity-9c27b56b516
*/
public class ShakeScript : MonoBehaviour
{
    // Desired duration of the shake effect
    private float shakeDuration = 0f;
    
    // A measure of magnitude for the shake. Tweak based on your preference
    private float shakeMagnitude = 0.7f;
    
    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 4.0f;
    
    // The initial position of the GameObject
    Vector3 initialPosition;

    void Awake(){
        initialPosition = transform.localPosition;
    }


    void Update(){
        if (shakeDuration > 0){
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else{
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake() {
        shakeDuration = 0.6f;
    }
}
