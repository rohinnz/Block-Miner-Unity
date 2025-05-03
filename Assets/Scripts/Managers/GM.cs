// Copyright (c) 2023 Rohin Knight
using UnityEngine;
using DG.Tweening;

/// <summary>
/// The Game Manager singleton.
/// References all other managers and initializes them when game starts.
/// </summary>
[DefaultExecutionOrder(-1)]
public class GM : MonoBehaviour
{
    public static GM Ins { get; private set; }
    public static LevelSerializer LevelSerializer => Ins.levelSerializer;
    public static BlockchainManager Blockchain => Ins.blockchainManager;
    public static SaveStateManager State => Ins.saveStateManager;
    public static ScreenManager Screens => Ins.screenManager;

    [SerializeField] LevelSerializer levelSerializer;
    [SerializeField] BlockchainManager blockchainManager;
    [SerializeField] SaveStateManager saveStateManager;
    [SerializeField] ScreenManager screenManager;

    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else if (Ins != this)
        {
            // We already have an instance of GM, so destroy this instance
            Destroy(this);
        }
    }

    private void Init()
    {
        DOTween.Init();
    }
}
