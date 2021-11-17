﻿using System.Collections;
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
    public string BGMAlternative;
    public GameObject lootList;
    public GameObject interactableList;
    public Transform healZoneTransform;

    /**
    * Ao iniciar a cena, lê informações do GameManager para instânciar os 
    * objetos corretamente na cena.
    */
    void Start()
    {
        GameManager.Instance.Init();
        if (!GameManager.Instance.EventList[(int)GameManager.Event.EnteredForest]){
            GameManager.Instance.EventList[(int)GameManager.Event.EnteredForest] = true;
            PlayerOverworld = Instantiate(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            SavePlayerObject();
        }
        else if(GameManager.Instance.state == GameManager.State.City){
            PlayerOverworld = Instantiate(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        }
        else
        {
            PlayerOverworld = Instantiate(PlayerPrefab, GameManager.Instance.overworldPlayerPosition, GameManager.Instance.overworldPlayerRotation);//acho que o que esta cagando aqui eh que ele ta pegando o GameData script e não o instanciado
        }
        GameManager.Instance.state = GameManager.State.Overworld;
        foreach (GameManager.LootAcquired l in GameManager.Instance.loot)
        {
            if(l.OverworldID == OverWorldID){
                foreach (OverworldLoot item in lootList.GetComponentsInChildren<OverworldLoot>())
                {
                    if(item.ID == l.ID){
                        Destroy(item.gameObject);
                    }
                }
            }
        }
        foreach (GameManager.InteractableUsed i in GameManager.Instance.interactables)
        {
            if(i.OverworldID == OverWorldID){
                foreach (Interactable item in interactableList.GetComponentsInChildren<Interactable>())
                {
                    if(item.ID == i.ID){
                        item.used = true;
                    }
                }
            }
        }
        PlayerOverworld.GetComponent<RandomEncouters>().OverWorldID = OverWorldID;
        GameManager.Instance.CurrentOverworldID = OverWorldID;
        if((BGMAlternative != "") && (Random.Range(0,4) == 0)){
            AudioManager.instance.Play(BGMAlternative);
        }
        else{
            AudioManager.instance.Play(BGM);
        }
        foreach (Unit u in GameManager.Instance.party.GetComponentsInChildren<Unit>())
        {
            if(u){
                u.HealVolatileCondition();
            }
        }
    }

    /**
    * Salva o estado atual do jogador para ser acessado de novo futuramente
    */
    public static void SavePlayerObject(){//depois isso deve virar um "SaveOverworldState()
        GameManager.Instance.SaveOverwoldPlayer(PlayerOverworld.transform);
    }

    public void SaveInitialPosition(){//Salva a localização do jogador no inicio do
        GameManager.Instance.SaveOverwoldPlayer(PlayerOverworld.transform);
    }

}
