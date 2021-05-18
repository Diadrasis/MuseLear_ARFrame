using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour// Singleton<ServerManager>
{

    //protected ServerManager() { }

    public VideoController[] videoControllers;

    //public void Init()
    //{
    //    if (B.isEditor) Debug.LogWarning("Server Manager Init");
    //}


    private void Awake()
    {
        videoControllers = FindObjectsOfType<VideoController>();
        
    }

    IEnumerator Start()
    {
        while (videoControllers.Length <= 0)
        {
            videoControllers = FindObjectsOfType<VideoController>();
            yield return new WaitForSecondsRealtime(0.2f);
        }

        bool shouldDownloadVideos = false;

        CoroutineWithData cd = new CoroutineWithData(this, CheckForUpdates());
        yield return cd.coroutine;

        if (B.isEditor)
        {
            Debug.LogWarning(cd.result.ToString());
            Debug.LogWarning(versionUpdate + " =? " + PlayerPrefs.GetInt("lastUpdateDate"));
        }

        if (cd.result.ToString() == "true")
        {
            if (PlayerPrefs.GetInt("lastUpdateDate") != versionUpdate)
            {
                shouldDownloadVideos = true;
            }
            PlayerPrefs.SetInt("lastUpdateDate", versionUpdate);
            PlayerPrefs.Save();
        }

        if (!shouldDownloadVideos)
        {
            foreach (VideoController v in videoControllers)
            {
                if(!Stathis.File_Manager.IfFileExist(v.GetVideoFileName(), out string path))
                {
                    CoroutineWithData cdVideo = new CoroutineWithData(this, DownloadVideo(v.GetVideoURL(), v.GetVideoFileName()));
                    yield return cdVideo.coroutine;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        else
        {
            foreach (VideoController v in videoControllers)
            {
                CoroutineWithData cdVideo = new CoroutineWithData(this, DownloadVideo(v.GetVideoURL(), v.GetVideoFileName()));
                yield return cdVideo.coroutine;
                yield return new WaitForEndOfFrame();
            }

        }

        yield return null;
    }

    string urlUpdateCheck = "https://stathis.stagegames.eu/arframe/";
    public int versionUpdate;

    IEnumerator CheckForUpdates()
    {
        UnityWebRequest www = UnityWebRequest.Get(urlUpdateCheck+"check.txt");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield return "fail";
        }
        else
        {
            if (int.TryParse(www.downloadHandler.text, out versionUpdate))
            {
                yield return "true";
            }
            else
            {
                yield return "false";
            }
        }
    }


    IEnumerator DownloadVideo(string videoURL, string fileName)
    {
        //byte[] newBytes = null;
        //try
        //{
        //    newBytes = File.ReadAllBytes(tempPath);
        //    if(B.isEditor) Debug.Log("Loaded " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
        //    yield return null;
        //}
        //catch (Exception e)
        //{
        //    if (B.isEditor) Debug.LogWarning("Failed To Load " + dataFileName + " from: " + tempPath.Replace("/", "\\"));
        //    if (B.isEditor) Debug.LogWarning("Error: " + e.Message);
        //}

        UnityWebRequest www = UnityWebRequest.Get(videoURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Stathis.File_Manager.SaveOfflineData(www.downloadHandler.data, fileName, Stathis.File_Manager.Ext.NULL);
        }

        yield return null;
    }


}
