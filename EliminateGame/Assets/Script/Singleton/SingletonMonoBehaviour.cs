using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    protected static T _classInstance;

    public static T instance
    {
        get
        {
            if (_classInstance == null)
            {
                GameObject go = GameObject.Find("SingletonMonoBehaviour");
                if (!go)
                {
                    go = new GameObject("SingletonMonoBehaviour");
                    DontDestroyOnLoad(go);
                }
                //if (SceneManager.sceneCount > 1)
                //{
                //    if (go.scene != SceneManager.GetSceneAt(1))
                //    {
                //        SceneManager.MoveGameObjectToScene(go, SceneManager.GetSceneAt(1));
                //    }
                //}
                _classInstance = go.AddComponent<T>();
                _classInstance.enabled = true;
            }
            return _classInstance;
        }
    }

    protected virtual void Awake()
    {
        _classInstance = GetComponent<T>();
    }

    void Start() { }

}
