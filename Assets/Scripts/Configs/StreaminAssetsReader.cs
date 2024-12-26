using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class StreaminAssetsReader
{
    private string BasePath => Application.streamingAssetsPath + "/";

    public async Task<T> ReadAsync<T>(string path)
    {
        string endPath = BasePath + path;
        string file;

#if UNITY_WEBGL
        file = await CreateWebRequestAwait(endPath);
#else
        file = File.ReadAllText(endPath);
#endif
        T fromJson = JsonUtility.FromJson<T>(file);
        return fromJson;
    }

    private async Task<string> CreateWebRequestAwait(string path)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(path);
        webRequest.SendWebRequest();

        while (webRequest.isDone == false)
        {
            await Task.Yield();
        }

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            return webRequest.downloadHandler.text;
        }
        else
        {
            throw new ArgumentNullException();
        }
    }
}
