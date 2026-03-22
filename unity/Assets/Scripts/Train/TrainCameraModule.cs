using Fusion;
using UnityEngine;

/// <summary>
/// 電車スポーン時のカメラ初期化（TrainNetworkState から切り出し）。
/// </summary>
public static class TrainCameraModule
{
    public static void ApplyInitialSetup(CameraController cameraController, NetworkRunner runner)
    {
        if (cameraController == null)
        {
            return;
        }

        if (runner != null)
        {
            cameraController.ApplyInitialCameraSetup(runner);
        }
        else
        {
            // Runnerがまだ設定されていない場合は、Runnerなしで試行
            cameraController.ApplyInitialCameraSetup();
        }
    }
}
