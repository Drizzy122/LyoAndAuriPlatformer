using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneController : MonoBehaviour
{
    public event Action<SceneFadingOutEventArgs> OnSceneFadingOut;

    [SerializeField]
    private Color _fadeColor;

    [SerializeField]
    private float _fadeDuration;

    private SceneFade _sceneFade;
    private bool _sceneLoading;

    private void Awake()
    {
        _sceneFade = GetComponentInChildren<SceneFade>();
    }

    private IEnumerator Start()
    {
        yield return _sceneFade.FadeInCoroutine(_fadeDuration, _fadeColor);
    }

    public void LoadScene(SceneIndex sceneIndex)
    {
        if (_sceneLoading == false)
        {
            _sceneLoading = true;
            StopAllCoroutines();
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }
    }

    private IEnumerator LoadSceneCoroutine(SceneIndex sceneIndex)
    {
        OnSceneFadingOut?.Invoke(new SceneFadingOutEventArgs(_fadeDuration));
        yield return _sceneFade.FadeOutCoroutine(_fadeDuration, _fadeColor);
        
        yield return SceneManager.LoadSceneAsync((int)sceneIndex);

        _sceneLoading = false;
    }
}
