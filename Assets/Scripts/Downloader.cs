using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;

public class Downloader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference reference = storage.GetReference("apo.jpg");

        string filePath = Application.dataPath + "/Images/apo.jpg";

        reference.GetFileAsync(filePath).ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("downloaded");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}