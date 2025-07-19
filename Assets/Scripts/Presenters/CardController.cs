using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Firebase;
using Firebase.Extensions;

public class CardController : MonoBehaviour
{
    [SerializeField] private CardInfoDisplay _cardInfoDisplay;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task => {
                if (task.Result == DependencyStatus.Available)
                {
                    // 取得して何かに使う、またはログ出力する
                    var app = FirebaseApp.DefaultInstance;
                    Debug.Log($"FirebaseInitialized: {app.Name}");
                }
                else
                {
                    Debug.LogError("Firebase 初期化エラー: " + task.Result);
                }
            });

    }

    public async UniTask OnCardGenerated(GeminiCardGenerator.CardData data, string title, Texture2D tex)
    {
        // 1) 画面に表示
        _cardInfoDisplay.DisplayCardData(data, title, tex);

        // 2) 画像アップロード
        string fileName = $"{Guid.NewGuid()}.png";
        string url = await StorageHelper.UploadTextureAsync(tex, fileName);
        
        // 3) 画像ID作成
        string imageId = Guid.NewGuid().ToString();

        // 3) モデルにマップ
        var record = new CardDatabase.CardRecord
        {
            authorId    = LocalUser.GetLocalUserId(),
            imageId     = imageId,
            title       = title,
            intellect   = data.intellect,
            athleticism = data.athleticism,
            luck        = data.luck,
            flavor_text = data.flavor_text,
            createdAt   = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            imageUrl    = url
        };

        // 4) RTDB に保存
        await CardDatabase.SaveCardAsync(record);
    }
}
