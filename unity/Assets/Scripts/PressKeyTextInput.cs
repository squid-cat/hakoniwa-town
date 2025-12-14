using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // TextMeshProの名前空間をインポート

public class DisplayKeyInput : MonoBehaviour
{
    public TextMeshPro textDisplay; // TextMeshProUGUIのテキストコンポーネントへの参照

    void Update()
    {
        var current = Keyboard.current;
        if (current == null) return; // キーボードが接続されていない場合は終了

        foreach (var keyControl in current.allKeys) {
            if (keyControl.wasPressedThisFrame) {
                textDisplay.text = $"Pressed Key: {keyControl.name}";
                break; // 最初に押されたキーだけを表示
            }
        }
    }
}
