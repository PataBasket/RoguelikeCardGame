// Assets/Scripts/ImageExtractor.cs
using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;                 // エディタ実行時用
#else
using SFB;                         // Standalone File Browser
#endif

public class ImageExtractor
{
    /// <summary>
    /// 画像を選択してTexture2Dを返す。
    /// onSuccess: 正常に読み込めたTexture2D
    /// onError  : エラーメッセージ
    /// </summary>
    public void ExtractImage(Action<Texture2D> onSuccess, Action<string> onError)
    {
        string path = null;

        #if UNITY_EDITOR
        // エディタ上ならUnityEditorのファイルダイアログを使う
        path = EditorUtility.OpenFilePanel("画像を選択", "", "png,jpg,jpeg");
        if (string.IsNullOrEmpty(path))
        {
            onError?.Invoke("ファイルが選択されませんでした");
            return;
        }
        #else
        // ビルド時はSFBプラグインを使う
        var extensions = new[] {
            new ExtensionFilter("画像ファイル", "png", "jpg", "jpeg")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("画像を選択", "", extensions, false);
        if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            onError?.Invoke("ファイルが選択されませんでした");
            return;
        }
        path = paths[0];
        #endif

        // 拡張子チェック
        string ext = Path.GetExtension(path).ToLower();
        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
        {
            onError?.Invoke("png/jpg/jpeg 以外のファイルは選択できません");
            return;
        }

        byte[] data;
        try
        {
            data = File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            onError?.Invoke($"ファイル読み込みに失敗: {e.Message}");
            return;
        }

        var tex = new Texture2D(2, 2);
        if (!tex.LoadImage(data))
        {
            onError?.Invoke("画像データのデコードに失敗しました");
            return;
        }

        // アスペクト比チェック (横:縦 = 4:3 → width/height = 4/3)
        float ratio = (float)tex.width / tex.height;
        const float targetRatio = 4f / 3f;
        if (Mathf.Abs(ratio - targetRatio) > 0.01f)
        {
            // 比率を小数点第2位まで丸めて表示
            string ratioStr = ratio.ToString("F2");
    
            // またはピクセル数での比を簡約して表示したい場合は、GCDで割り算してもOK
            int w = tex.width;
            int h = tex.height;
            int gcd = GreatestCommonDivisor(w, h);
            string simplified = $"{w/gcd}:{h/gcd}";

            onError?.Invoke(
                $"画像比率が縦3:横4ではありません\n"
                // $"実際の比率: {ratioStr} (幅:高さ = {simplified})"
            );
            return;
        }

        onSuccess?.Invoke(tex);
    }

    // GCDを計算するヘルパー
    private int GreatestCommonDivisor(int a, int b)
    {
        return b == 0 ? a : GreatestCommonDivisor(b, a % b);
    }
}
