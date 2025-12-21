using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using TMPro;

public class TrainControllerManger : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private TextMeshProUGUI _loadingText;

    private void Start()
    {
        _loadingText.gameObject.SetActive(false);

        GenerateQRCode("https://github.com/squid-cat");
    }

    public void GenerateQRCode(string url)
    {
        _loadingText.gameObject.SetActive(true);

        var texture = new Texture2D(256, 256);
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 256,
                Width = 256,
                Margin = 1
            }
        };

        var pixelData = writer.Write(url);
        texture.SetPixels32(pixelData);
        texture.Apply();

        _loadingText.gameObject.SetActive(false);

        _rawImage.texture = texture;
    }
}
