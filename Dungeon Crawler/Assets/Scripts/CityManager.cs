using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CityManager : MonoBehaviour
{
    void Start(){
        GameManager.Instance.Init();
    }
    public void ChangeScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
