using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;
namespace ClientComponents.Base
{
    public class TSingleton<T>
        where T : class, new()
    {
        static T ms_instance = null;
        public static T Me
        {
            get
            {
                if (null == ms_instance)
                {
                    ms_instance = new T();
                }
                return ms_instance;
            }
        }
    }

    public class TSingletonX<T>
        where T : class, new()
    {
        static T ms_instace = null;
        public static void CreateSingleton()
        {
            if (null == ms_instace)
            {
                ms_instace = new T();
            }
        }

        public static void DestroySingleton()
        {
            if (ms_instace != null)
            {
                ms_instace = null;
            }
        }

        public static T Me
        {
            get
            {
                return ms_instace;
            }
        }
    }

    public class TMonoSingleton<T> : UnityEngine.MonoBehaviour where T : TMonoSingleton<T>
    {
        private static T _instance;
        private static object _lock = new object();
        public virtual void Awake() { }
        public static T Me
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));
                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return _instance;
                        }
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                    return _instance;
                }
            }
        }
        private static bool applicationIsQuitting = false;
        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}