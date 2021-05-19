using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Classe responsável por gerenciar as cenas de mundo aberto.
*/

public class GameHandler_Overmap : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PlayerPrefab;
    private static GameObject PlayerOverworld;

    public int OverWorldID;
    public string BGM;

    /**
    * Ao iniciar a cena, lê informações do GameManager para instânciar os 
    * objetos corretamente na cena.
    */
    void Start()
    {
        if (!GameManager.Instance.GetIsInit()){
            GameManager.Instance.Init();
            PlayerOverworld = Instantiate(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            GameManager.Instance.state = GameManager.State.Overworld;
            SavePlayerObject();
        }
        else
        {
            PlayerOverworld = Instantiate(PlayerPrefab, GameManager.Instance.overworldPlayerPosition, GameManager.Instance.overworldPlayerRotation);//acho que o que esta cagando aqui eh que ele ta pegando o GameData script e não o instanciado
        }
        PlayerOverworld.GetComponent<RandomEncouters>().OverWorldID = OverWorldID;
        AudioManager.instance.Play(BGM);
    }

    /**
    * Salva o estado atual do jogador para ser acessado de novo futuramente
    */
    public static void SavePlayerObject(){//depois isso deve virar um "SaveOverworldState()
        GameManager.Instance.SaveOverwoldPlayer(PlayerOverworld.transform);
    }

}
