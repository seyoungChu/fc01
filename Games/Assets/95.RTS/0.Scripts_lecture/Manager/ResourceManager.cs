using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMonobehaviour<ResourceManager> 
{
    /// <summary>
    /// Load the specified path.
    /// </summary>
    public Object Load(string path)
    {
        return Resources.Load(path);
    }

    /// <summary>
    /// Loadands the instantiate.
    /// </summary>
    public GameObject LoadandInstantiate(string path)
    {
        Object source = this.Load(path);
        if(source != null)
        {
			return Instantiate(this.Load(path)) as GameObject;         
        }
        else
        {
            return null;
        }
    }
	
}
