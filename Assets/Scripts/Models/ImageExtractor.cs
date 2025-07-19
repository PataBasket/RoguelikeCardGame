using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#elif UNITY_IOS || UNITY_ANDROID
using NativeGallery;  // ネイティブギャラリー
#else
using SFB;            // StandaloneFileBrowser
#endif

public class ImageExtractor
{
    public void ExtractImage(Action<Texture2D> onSuccess, Action<string> onError)
    {
#if UNITY_EDITOR
        var path = EditorUtility.OpenFilePanel("画像を選択", "", "png,jpg,jpeg");
        if (string.IsNullOrEmpty(path))
        {
            onError?.Invoke("ファイルが選択されませんでした");
            return;
        }
        LoadTexture(path, onSuccess, onError);

#elif UNITY_IOS || UNITY_ANDROID
        // ネイティブギャラリーを呼び出し
        Permission perm = NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                onError?.Invoke("ファイルが選択されませんでした");
                return;
            }
            LoadTexture(path, onSuccess, onError);
        }, "Select Image", "image/*" );
        if (perm == Permission.Denied)
            onError?.Invoke("ギャラリーへのアクセスが拒否されました");

#else
        // デスクトップ向け：SFB
        var extensions = new[]{ new ExtensionFilter("画像ファイル","png","jpg","jpeg") };
        var paths = StandaloneFileBrowser.OpenFilePanel("画像を選択","", extensions, false);
        if (paths == null || paths.Length == 0)
        {
            onError?.Invoke("ファイルが選択されませんでした");
            return;
        }
        LoadTexture(paths[0], onSuccess, onError);
#endif
    }

    private void LoadTexture(string path, Action<Texture2D> onSuccess, Action<string> onError)
    {
        try
        {
            var data = File.ReadAllBytes(path);
            var tex  = new Texture2D(2, 2);
            if (!tex.LoadImage(data))
            {
                onError?.Invoke("画像データのデコードに失敗しました");
                return;
            }
            onSuccess?.Invoke(tex);
        }
        catch (Exception e)
        {
            onError?.Invoke($"ファイル読み込みに失敗: {e.Message}");
        }
    }
}
