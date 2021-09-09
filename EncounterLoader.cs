using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterLoader : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField]
    GameObject enemySpawns;
    [SerializeField]
    GameObject playerSpawn;

    EncounterList encounterList;
    EncounterList.Encounter encounter;

    EntityList entityList;
    [SerializeField]
    GameObject entityPrefab;
    Dictionary<Entity, int> entities = new Dictionary<Entity, int>();
    List<KeyValuePair<int, Entity>> sortedList = new List<KeyValuePair<int, Entity>>();

    [SerializeField]
    GameObject canvas;
    GameObject turnOrderBox;
    [SerializeField]
    GameObject inititivePopPrefab;

    [SerializeField]
    GameObject newHealthBar;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        encounterList = GetComponent<EncounterList>();
        entityList = GetComponent<EntityList>();
        turnOrderBox = canvas.transform.GetChild(1).gameObject;
    
        string character = "Sheriff"; //change to index id after character select is added
        Sprite spriteTemp;
        encounter = encounterList.GetEncounter(gameManager.encounterNum);

        //positions player on the player spawn point and assigns the correct 
        GameObject playerTemp = Instantiate(entityPrefab, playerSpawn.transform.GetChild(0).position, Quaternion.Euler(0, 0, 0), playerSpawn.transform.GetChild(0));
        spriteTemp = Resources.Load<Sprite>("Characters/" + character);
        playerTemp.GetComponent<SpriteRenderer>().sprite = spriteTemp;
        playerTemp.GetComponent<Entity>().Initialise(entityList.GetEntity(900)); //change '900' to index id after character select is added
        playerTemp.tag = "Player";

        //positions enemies from the given encounter and scales them to create false perspective
        for (int i = 0; i < encounter.entities.Count; i++)
        {
            GameObject entityTemp = Instantiate(entityPrefab, enemySpawns.transform.GetChild(i).position, Quaternion.Euler(0,0,0), enemySpawns.transform.GetChild(i));
            spriteTemp = Resources.Load<Sprite>("Enemies/" + encounter.entities[i].GetName());
            entityTemp.GetComponent<SpriteRenderer>().sprite = spriteTemp;
            entityTemp.GetComponent<SpriteRenderer>().sortingOrder = 4 - i;
            entityTemp.GetComponent<Entity>().Initialise(encounter.entities[i]);
            entityTemp.transform.localScale = new Vector3(1 - i * 0.2f, 1 - i * 0.2f, 1f);
        }

        //visuals ready, remove curtain
        gameManager.loadingScreen.SetActive(false);

        foreach (Entity entity in FindObjectsOfType<Entity>())
        {
            entities.Add(entity, entity.RollInitiative());
        }

        SortTurnOrder();

        StartCoroutine("SpawnInitiatives");
    }

    private void SortTurnOrder()
    {
        while (entities.Count > 0)
        {
            Entity highestEntity = FindObjectOfType<Entity>();

            int highest = 0;
            int temp = 0;

            foreach (Entity entity in FindObjectsOfType<Entity>())
            {
                entities.TryGetValue(entity, out temp);

                if (temp > highest)
                {
                    highest = temp;
                    highestEntity = entity;
                }
            }
            sortedList.Add(new KeyValuePair<int, Entity>(highest, highestEntity));
            entities.Remove(highestEntity);
        }
    }


    //spawns numbers displaying initiative rolls in order of highest, then adds them visually to the UI
    IEnumerator SpawnInitiatives()
    {
        int i = 0;
        foreach (KeyValuePair<int, Entity> entity in sortedList)
        {
            Sprite spriteTemp;
            GameObject newNumber = Instantiate(inititivePopPrefab, canvas.transform);
            Vector3 rectPos;

            //finds position of the character and displays the initiative above their heads
            rectPos = FindObjectOfType<Camera>().WorldToScreenPoint(entity.Value.gameObject.transform.position + 8 * Vector3.up + 1.1f * Vector3.down * (entity.Value.gameObject.transform.position.y - entity.Value.gameObject.transform.parent.parent.position.y));
            newNumber.GetComponent<RectTransform>().position = rectPos;
            newNumber.GetComponent<Text>().text = "" + entity.Key;
            newNumber.GetComponent<Text>().fontSize = Mathf.RoundToInt(30 - 6 * (entity.Value.gameObject.transform.position.y - entity.Value.gameObject.transform.parent.parent.position.y));

            //get character headshot
            if (entity.Value.transform.tag == "Player")
            {
                spriteTemp = Resources.LoadAll<Sprite>("Characters/" + entity.Value.entityName)[1];
            }
            else
            {
                spriteTemp = Resources.LoadAll<Sprite>("Enemies/" + entity.Value.entityName)[1];
            }

            //add headshot to UI and make it more transparent the further down the list it is.
            turnOrderBox.transform.GetChild(i).GetComponent<Image>().sprite = spriteTemp;
            Color tempColour = turnOrderBox.transform.GetChild(i).GetComponent<Image>().color;
            tempColour.a = 1 - (i / 6);
            turnOrderBox.transform.GetChild(i).GetComponent<Image>().color = tempColour;

            i++;
            yield return new WaitForSeconds(0.5f);
        }

        new WaitForSeconds(3f);

        //add healthbar and set health from entity list
        foreach (KeyValuePair<int, Entity> entity in sortedList)
        {
            GameObject healthBar = Instantiate(newHealthBar, canvas.transform.GetChild(2));
            Vector3 rectPos = FindObjectOfType<Camera>().WorldToScreenPoint(entity.Value.gameObject.transform.position + 8 * Vector3.up + 1.1f * Vector3.down * (entity.Value.gameObject.transform.position.y - entity.Value.gameObject.transform.parent.parent.position.y));
            healthBar.GetComponent<RectTransform>().position = rectPos;
            entity.Value.SetHealthBar(healthBar.GetComponent<HealthBar>());
        }

        Encounter newEncounter = gameObject.AddComponent<Encounter>();
        newEncounter.StartEncounter(sortedList, canvas);
        Destroy(this);
    }
}
