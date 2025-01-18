using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake() {
        if (instance != null) {
            Debug.LogWarning("Singleton instance already exists, destroying this instance.");
            Destroy(gameObject);
        }
        else {
            instance = (T)this;
        }
    }

    protected virtual void OnDestroy() {
        if (instance == this) instance = null;
    }
}