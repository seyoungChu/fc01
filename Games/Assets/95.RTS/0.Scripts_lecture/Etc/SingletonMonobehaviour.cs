using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    public static T Instance
    {
        get
        {
			
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
				//Debug.LogWarning(typeof(T).Name);
                if (_instance == null)
                {
                    var _gameObject = new GameObject(typeof(T).ToString());
                    _instance = _gameObject.AddComponent<T>();
                }

            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        //씬이 바껴도 이 게임오브젝트는 자동으로 사라지지 않습니다.
        DontDestroyOnLoad(transform.root);
    }

}
