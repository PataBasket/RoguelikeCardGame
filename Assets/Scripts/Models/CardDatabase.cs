using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using System;

public static class CardDatabase
{
    private static DatabaseReference Root => FirebaseDatabase.DefaultInstance.RootReference;

    public static async UniTask SaveCardAsync(CardRecord record)
    {
        // /cards/<autoPushKey> に JSON をセット
        var cardsRef = Root.Child("cards");
        var newRef   = cardsRef.Push();
        string json = JsonUtility.ToJson(record);
        await newRef.SetRawJsonValueAsync(json);
    }
    
    // このメソッド呼べばカード取ってこれる
    public static async UniTask<List<CardRecord>> LoadMyCardsAsync()
    {
        var myId    = LocalUser.GetLocalUserId();
        var cardsRef = Root.Child("cards");
        var query    = cardsRef.OrderByChild("authorId").EqualTo(myId);
        var snapshot = await query.GetValueAsync();

        var list = new List<CardRecord>();
        foreach (var child in snapshot.Children)
        {
            var record = JsonUtility.FromJson<CardRecord>(child.GetRawJsonValue());
            list.Add(record);
        }
        return list;
    }
    

    [Serializable]
    public class CardRecord
    {
        public string authorId;
        public string title;
        public int intellect;
        public int athleticism;
        public int luck;
        public string flavor_text;
        public string imageUrl;  // ← ここに Storage の URL を格納
        public long createdAt; 
    }
}