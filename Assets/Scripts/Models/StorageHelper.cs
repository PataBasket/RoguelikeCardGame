using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public static class StorageHelper
{
    /// <summary>
    /// Texture2D を PNG に変換して Firebase Storage にアップロードし、公開 URL を文字列で返す
    /// </summary>
    public static async Task<string> UploadTextureAsync(Texture2D tex, string fileName)
    {
        // バイト配列化
        byte[] png = tex.EncodeToPNG();

        // Storage ルート参照
        var storage = FirebaseStorage.DefaultInstance;
        var imagesRef = storage.GetReference("cardImages/" + fileName);

        // メタデータ（Content-Type）を付与（任意）
        var meta = new MetadataChange { ContentType = "image/png" };

        // アップロード
        await imagesRef.PutBytesAsync(png, meta);

        // ダウンロード URL を取得（Uri 型）
        Uri downloadUri = await imagesRef.GetDownloadUrlAsync();

        // 文字列として返す
        return downloadUri.ToString();
    }
}