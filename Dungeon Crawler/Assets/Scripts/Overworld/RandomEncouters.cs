using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



/**
* Gerência os RandomEncounters que ocorrem no Overworld.
*/
public class RandomEncouters : MonoBehaviour
{
    private int Encounter_Counter = 0;
    private int Max_Counter = 64;

    private string zoneID;

    public int OverWorldID;
 
    private PlayerController playerControllerScript;
    public TextAsset jsonFile;

    [System.Serializable]
    public class encounter
    {
        public int min_level;
        public int max_level;
        public List<string> enemy_formation;

        public encounter(int min, int max, List<string> formation){
            min_level = min;
            max_level = max;
            enemy_formation = formation;
        }
    }
    [System.Serializable]
    public class zoneData{
        public string zoneID;
        public List<encounter> encounterList;
    }
    [System.Serializable]
    public class EncounterTable{
        public List<zoneData> table;
    }
    EncounterTable loadedEncounterTable;
    public void Start(){
        loadedEncounterTable = JsonUtility.FromJson<EncounterTable>(jsonFile.text);
        playerControllerScript = gameObject.GetComponent<PlayerController>();
    }

    /**
    * Incrementa o Encounter_Counter e chama o Check_Encounter
    *
    * @param EncounterRate Valor pelo qual o Encounter_Counter deve ser implementado
    */
    public void Increment_Encouter(int EncounterRate, string zoneId){
        Encounter_Counter += EncounterRate;
        zoneID = zoneId; 
        Check_Encounter();
    }

    /**
    * Checa se o Encounter_Counter chegou ao valor máximo, 
    * caso tenha ultrapassado o valor máximo chama o SetUpBattleScene() e zera o Encounter_Counter.
    *
    * @param EncounterRate Valor pelo qual o Encounter_Counter deve ser implementado
    */
    private void Check_Encounter(){
        if (Encounter_Counter >= Max_Counter)
        {
            SetUpBattleScene();
            Encounter_Counter = 0;
        }
    }

    private void SetUpBattleScene(){
        //set up the battle scene here
        //Search EncounterTable(playerControllerScript.GetZoneID());
        //load battlescene
        //SceneManager.LoadScene("scene2", parameters); Loads scene with parameters, Eu acho que deve ser assim que vou chamar a battlescene passando a party do jogador e o grupo de montros corretos
        GameHandler_Overmap.SavePlayerObject();
        GameManager.Instance.CurrentOverworldScene = SceneManager.GetActiveScene().buildIndex;
        GameManager.Instance.state = GameManager.State.InRandomEncounter;
        GameManager.Instance.SaveEncounter(SelectEnemyEncounter());
        //acho que mais pra frente essa pedaço de escolher qual é a cena de batalha deve ser feita usando um json
        switch (OverWorldID)
        {
            case 1:
                GameManager.Instance.battleID = GameManager.BattleID.RandomBattle1;
                SceneManager.LoadScene("Battle Scene");
            break;
            default:
            break;
        }
    }

    private encounter SelectEnemyEncounter(){
        System.Random random = new System.Random();
        //percorre o EncounterTable para achar o zoneData referente ao zoneID atual,e seleciona um Encounter aleatório do encounter list
        foreach (zoneData zonedata in loadedEncounterTable.table)
        {
            if(zonedata.zoneID == zoneID){
                return zonedata.encounterList[random.Next(zonedata.encounterList.Count)];
            }
        }
        Debug.Log("Retornou Null por algum motivo\n");
        return null;
    }
}
