using DG.Tweening;
using DuckReaction.Common;
using DuckReaction.Common.Container;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneService : MonoBehaviour
{
    [SerializeField]
    string[] _firstScenes = { "Scenes/Home" };

    [SerializeField]
    Image _imageEffect;

    [ShowInInspector, ReadOnly]
    float _targetY = 500;

    [SerializeField]
    float _transitionDuration = 0.5f;

    List<string> _scenesToUnload;
    List<string> _scenesToLoad;
    bool _inTransition = false;
    RectTransform _rect;

    void Start()
    {
        _targetY = Screen.height;
        _imageEffect.color = Color.black;
        _rect = _imageEffect.GetRectTransform();
        Vector2 target = _rect.anchoredPosition;
        target.y = _targetY;

        if (SceneManager.sceneCount == 1)
            StartSceneTransition(new string[0], _firstScenes, false);
        else
            TransitionEndAnimation();
    }

    private void Update()
    {
        if (!_inTransition && _targetY != Screen.height)
        {
            _targetY = Screen.height;
            var pos = _rect.anchoredPosition;
            pos.y = _targetY;
            _rect.anchoredPosition = pos;
        }
    }

    public void StartSceneTransition(string[] scenesToUnload, string[] scenesToLoad, bool fadeIn = true)
    {
        _inTransition = true;
        _scenesToUnload = new List<string>(scenesToUnload);
        _scenesToLoad = new List<string>(scenesToLoad);

        if (fadeIn)
            TransitionStartAnimation();
        else
            ProcessNextScene();
    }

    private void OnSceneLoadedOrUnloaded(AsyncOperation op)
    {
        ProcessNextScene();
    }

    private void ProcessNextScene()
    {
        if (_scenesToUnload.Count == 0 && _scenesToLoad.Count == 0)
        {
            TransitionEndAnimation();
        }
        else if (_scenesToUnload.Count == 0)
        {
            LoadNextScene();
        }
        else
        {
            UnloadNextScene();
        }
    }

    private void UnloadNextScene()
    {
        var scene = _scenesToUnload.Shift();
        SceneManager.UnloadSceneAsync(scene).completed += OnSceneLoadedOrUnloaded;
    }

    private void LoadNextScene()
    {
        var scene = _scenesToLoad.Shift();
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive).completed += OnSceneLoadedOrUnloaded;
    }

    [ContextMenu("Test transition start animation")]
    private void TransitionStartAnimation()
    {
        _imageEffect.rectTransform.DOAnchorPosY(0, _transitionDuration).SetEase(Ease.OutCubic).onComplete += ProcessNextScene;
    }

    [ContextMenu("Test transition end animation")]
    private void TransitionEndAnimation()
    {
        _imageEffect.rectTransform.DOAnchorPosY(_targetY, _transitionDuration).SetEase(Ease.OutCubic).onComplete += OnTransitionComplete;
    }

    private void OnTransitionComplete()
    {
        Debug.Log("Transition complete");
        _inTransition = false;
    }
}
