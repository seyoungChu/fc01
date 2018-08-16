using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonobehaviour<DataManager>
{

    public EntityTable entityTable = null;
    public Dictionary<int, EntityData> entityData = new Dictionary<int, EntityData>();
	public SoundData soundData = null;
    private void Start()
    {

        entityTable = (EntityTable)Resources.Load("Data/EntityTable");
        int i = 0;
        foreach (EntityTable.Sheet s in entityTable.sheets)
        {
            foreach (EntityTable.Param p in s.list)
            {
                Debug.LogWarning(i + " /  " + p.ID + " / " + p.EntityCategory.ToString() + " / " + p.EntityType.ToString() + "/" + p.Prefab);
                entityData.Add(p.ID, new EntityData(p.ID, p.EntityCategory, p.EntityType, p.HP, p.Level, p.AttackPower, p.SearchRange, p.AttackSpeed, p.Prefab));
                i++;
            }
        }

		if (soundData == null) 
		{
			soundData = ScriptableObject.CreateInstance<SoundData>();
			soundData.LoadData();
		}

    }

    public EntityData GetEntityData(int ID)
    {
        if (this.entityData.ContainsKey(ID))
        {
            return this.entityData[ID];
        }
        else
        {
            return null;
        }
    }

    public EntityData GetEntityData(EntityCategory category)
    {
        foreach (KeyValuePair<int, EntityData> kv in entityData)
        {
            if (kv.Value.entCategory == category)
            {
                return kv.Value;
            }
        }
        return null;
    }

	public static SoundData SoundData()
    {
        if (DataManager.Instance.soundData == null)
        {
            DataManager.Instance.soundData = ScriptableObject.CreateInstance<SoundData>();
            DataManager.Instance.soundData.LoadData();
        }
        return DataManager.Instance.soundData;
    }

}
