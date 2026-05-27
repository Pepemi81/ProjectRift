using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static T instance { get; private set; }
    protected virtual bool IsPersistent => false;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this as T;

            if (IsPersistent)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}