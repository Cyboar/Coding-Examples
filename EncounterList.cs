using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LoadOrder))]
public class EncounterList : MonoBehaviour
{
    EntityList entityList;

    //encounters can have up to three enemies
    public class Encounter
    {
        public List<EntityList.Entity> entities = new List<EntityList.Entity>();

        public Encounter(EntityList.Entity entity1)
        {
            entities.Add(entity1);
        }

        public Encounter(EntityList.Entity entity1, EntityList.Entity entity2)
        {
            entities.Add(entity1);
            entities.Add(entity2);
        }

        public Encounter(EntityList.Entity entity1, EntityList.Entity entity2, EntityList.Entity enemy3)
        {
            entities.Add(entity1);
            entities.Add(entity2);
            entities.Add(enemy3);
        }
    }


    Dictionary<int, Encounter> encounters = new Dictionary<int, Encounter>();
    int index = 0;

    public void Load()
    {
        entityList = GetComponent<EntityList>();

        encounters.Add(Index(), new Encounter(entityList.GetEntity(1), entityList.GetEntity(2), entityList.GetEntity(3)));
        encounters.Add(Index(), new Encounter(entityList.GetEntity(3), entityList.GetEntity(2), entityList.GetEntity(3)));
        encounters.Add(Index(), new Encounter(entityList.GetEntity(2), entityList.GetEntity(1), entityList.GetEntity(1)));

        /*
        foreach(Encounter encounter in encounters)
        {
            print("ENCOUNTER " + encounter.id + ": ");
            foreach(EnemyList.Enemy e in encounter.enemy)
            {
                print(" " + e.id + " - " + e.name + " |");
            }
            print("\n");
        }
        */
    }

    int Index(){
        index += 1;
        return index;
    }

    public Encounter GetEncounter(int id)
    {
        Encounter temp = null;
        if (encounters.TryGetValue(id, out temp))
        {
            //success!
            return temp;
        }
        else
        {
            //failure!
            Debug.Log("An encounter for the given ID (" + id + ") cannot be found.");
            return null;
        }
    }
}
