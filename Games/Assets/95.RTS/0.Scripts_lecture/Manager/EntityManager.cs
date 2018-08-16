using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMonobehaviour<EntityManager>
{

    public List<Entity> entities = new List<Entity>();

    // Update is called once per frame
    public void UpdateEntities()
    {
        if (entities.Count == 0)
        {
            return;
        }
        for (int i = 0; i < entities.Count; ++i)
        {
            this.entities[i].UpdateEntity();
        }
    }
    //temp!
    //private void Start()
    //{
    //    Building[] buildings = FindObjectsOfType<Building>();
    //    if (buildings == null || buildings.Length == 0)
    //    {
    //        return;
    //    }
    //    for (int i = 0; i < buildings.Length; i++)
    //    {
    //        if (entities.Contains(buildings[i]) == false)
    //        {
    //            buildings[i].CashingObject();
    //            entities.Add(buildings[i]);
    //        }
    //    }
    //}


    public void SpawnEntity(int ID)
    {

        EntityData spawnData = DataManager.Instance.GetEntityData(ID);
        if (spawnData != null)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f));//InputManager.Instance.GetCurrentInput().currentRayHitPosition;
            pos.z = -1.0f;

            GameObject spawnObj = Instantiate(Resources.Load(spawnData.Prefab), pos, Quaternion.identity) as GameObject;
            Entity newEntity = spawnObj.GetComponent<Entity>();
            newEntity.CashingObject();
            newEntity.InitEntity(spawnData);
            if (this.entities.Contains(newEntity) == false)
            {
                entities.Add(newEntity);
            }

        }
    }

    public Transform GetCloseEntity(Vector3 searchPoint, float range, EntityType wantedType)
    {
        if (this.entities == null || this.entities.Count == 0)
        {
            return null;
        }

        Transform entity = null;

        for (int i = 0; i < this.entities.Count; i++)
        {
            if (this.entities[i] == null || this.entities[i].IsDead() == true)
            {
                continue;
            }

            if (this.entities[i].entitytype == wantedType)
            {
                float dist = (this.entities[i].transform.position - searchPoint).magnitude;
                if (dist <= range)
                {
                    range = dist;
                    entity = this.entities[i].transform;
                }
            }
        }

        return entity;
    }
}
