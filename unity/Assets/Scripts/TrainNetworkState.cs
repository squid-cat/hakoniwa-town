using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class TrainNetworkState : NetworkBehaviour
{
    [Networked]
    public int CurrentNotch { get; set; } = 0;
    [Networked] public float CurrentSpeed { get; set; } = 0f;

    [SerializeField] private SplineAnimate _splineAnimate;
    [SerializeField] private SplineContainer splineContainer;

    [SerializeField] private float _accelerationUnit = 2.0f; // ノッチ1あたりの加速度
    [SerializeField] private float _friction = 0.1f;        // 摩擦係数
    [SerializeField] private float _maxSpeed = 20f;         // 最高速度

    [SerializeField] private CameraController _cameraController;
    [SerializeField] private Slider _notchSlider;

    [SerializeField] private List<TextMeshProUGUI> _TrainInfoTextList = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> _TrainDebugInfoTextList = new List<TextMeshProUGUI>();

    private TrainSimulationModule _simulation;
    private TrainHudModule _hud;

    private void Awake()
    {
        _simulation = new TrainSimulationModule(
            _splineAnimate,
            splineContainer,
            _accelerationUnit,
            _friction,
            _maxSpeed);

        _hud = new TrainHudModule(
            this,
            _simulation,
            _notchSlider,
            _TrainInfoTextList,
            _TrainDebugInfoTextList,
            _accelerationUnit,
            _friction,
            _maxSpeed);
    }

    public override void Spawned()
    {
        _hud.OnSpawnedInitialSlider(CurrentNotch);
        StartCoroutine(DeferredSpawnedSetup());
    }

    /// <summary>
    /// WebGL ではスポーン直後の描画・シーン統合が追いつかないので、1 フレーム後に初期化する。
    /// </summary>
    private IEnumerator DeferredSpawnedSetup()
    {
        // 1フレーム待つ関連の処理
        yield return null;
        if (!Object || !Object.IsValid) yield break;

        TrainCameraModule.ApplyInitialSetup(_cameraController, Runner);
        _simulation.CacheTotalSplineLength();
    }

    /// <summary>HUD の Slider から RPC でノッチを送る。</summary>
    public void RequestSetNotchFromHud(int notch)
    {
        RPC_SetNotch(notch);
    }

    // RPCを使用してnotchを同期
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetNotch(int notch)
    {
        CurrentNotch = notch;
        _hud?.SyncPreviousNotchAfterRpc(CurrentNotch);
        Debug.Log($"[TrainNetworkState] CurrentNotch updated via RPC to: {CurrentNotch}");

        // Sliderの更新はUpdateメソッドで行う（RPC内では更新しない）
    }

    private void OnDestroy()
    {
        _hud?.OnDestroyUnregisterSlider();
    }

    public override void FixedUpdateNetwork()
    {
        if (_splineAnimate == null || !Object || !Object.IsValid) return;

        float speed = CurrentSpeed;
        _simulation.FixedUpdateStep(ref speed, CurrentNotch, Time.deltaTime);
        CurrentSpeed = speed;
    }

    private void Update()
    {
        _hud.UpdateFrame();
    }
}
