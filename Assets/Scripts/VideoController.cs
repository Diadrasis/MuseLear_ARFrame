using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer video;
    Material mat;
    AudioSource audioSource;
    public bool isUsingFile;
    public GameObject gbLoading;

    string videoURL = "https://stathis.stagegames.eu/arframe/";

    public enum VideoName { NULL, ARPolis, Athens, Culture, ErechtheioNormal, ErechtheioWithImage,
                            ParthenonNormal, ParthenonWithImage, SeaSun, Salonica }
    public VideoName videoName = VideoName.NULL;

    void Awake()
    {
        if (gbLoading == null)
        {
            gbLoading = transform.parent.Find("Canvas_Loading").gameObject;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.playOnAwake = false;

        video = GetComponent<VideoPlayer>();
        mat = GetComponent<Renderer>().material;
        mat.color = new Color(1, 1, 1, 0.0f);
        video.prepareCompleted += PrepareCompleted;
    }

    private void Start()
    {
        if (!isUsingFile)
        {
            if (videoName != VideoName.NULL)
            {
                video.source = VideoSource.Url;
                if (Stathis.File_Manager.IfFileExist(GetVideoName(videoName), out string path))
                {
                    video.url = path;
                }
                else
                {
                    video.url = videoURL + GetVideoName(videoName);
                }
            }
        }
        video.playOnAwake = true;
    }

    public string GetVideoURL()
    {
        return videoURL + GetVideoName(videoName);
    }

    public string GetVideoFileName()
    {
        return GetVideoName(videoName);
    }

    private void OnEnable()
    {
        if(B.isEditor) Debug.Log("OnEnable "+transform.parent.name);

        if (!isUsingFile)
        {
            if (gbLoading) gbLoading.SetActive(true);

            if (Stathis.File_Manager.IfFileExist(GetVideoName(videoName), out string path))
            {
                video.url = path;
            }
            else
            {
                video.url = videoURL + GetVideoName(videoName);
            }
        }
        //if (gbLoading) gbLoading.SetActive(true);
        mat.color = new Color(1, 1, 1, 0.0f);
    }

    private void OnDisable()
    {
        if (gbLoading) gbLoading.SetActive(false);
        mat.color = new Color(1, 1, 1, 0.0f);
    }

    IEnumerator DelayPlay()
    {
        //Debug.Log("DelayPlay " + transform.parent.name);
        yield return new WaitForSeconds(0.1f);

        float val = 0f;

        while (val < 0.9f)
        {
            val += Time.deltaTime * 2f;
            mat.color = new Color(1, 1, 1, val);
            yield return null;
        }

        //Debug.Log("play " + transform.parent.name);

        //video.time = 0.1f;
        video.Play();
        if (audioSource) audioSource.Play();
        mat.color = new Color(1, 1, 1, 1.0f);

        yield break;
    }

    void PrepareCompleted(VideoPlayer vp)
    {
        //Debug.Log("PrepareCompleted");

        if (gbLoading) gbLoading.SetActive(false);

        StartCoroutine(DelayPlay());
    }

    string GetVideoName(VideoName videoName)
    {
        switch (videoName)
        {
            case VideoName.NULL:
                return "";
            case VideoName.ARPolis:
                return "ARPolis.mp4";
            case VideoName.Athens:
                return "AthenswithTitle.mp4";
            case VideoName.Culture:
                return "CulturewithTitle.mp4";
            case VideoName.ErechtheioNormal:
                return "erechtheio_normal.mp4";
            case VideoName.ErechtheioWithImage:
                return "erechtheioWithImage.mp4";
            case VideoName.ParthenonNormal:
                return "ParthenonNoImage.mp4";
            case VideoName.ParthenonWithImage:
                return "ParthenonWithImageFixed.mp4";
            case VideoName.SeaSun:
                return "SeaSunwithTitle.mp4";
            case VideoName.Salonica:
                return "ThessalonikiwithTitle.mp4";
            default:
                return "";
        }
    }

    
}
