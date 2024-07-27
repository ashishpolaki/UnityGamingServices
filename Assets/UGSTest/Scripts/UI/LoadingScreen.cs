using System;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreenObject;

    // Singleton instance
    public static LoadingScreen Instance { get; private set; }

    private void Awake()
    {
        // Ensure there's only one instance of this class in the scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Show()
    {
        loadingScreenObject.SetActive(true);
    }
    public void Hide()
    {
        loadingScreenObject.SetActive(false);
    }
    public async Task PerformAsyncWithLoading(Func<Task> asyncOperation)
    {
        try
        {
            Show();
            await asyncOperation();
        }
        finally
        {
           Hide();
        }
    }

    public async Task<T> PerformAsyncWithLoading<T>(Func<Task<T>> asyncOperation)
    {
        try
        {
            Show();
            T result = await asyncOperation();
            return result;
        }
        finally
        {
            Hide();
        }
    }
}
