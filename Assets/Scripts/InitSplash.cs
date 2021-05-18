using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSplash : MonoBehaviour
{
    public delegate void SplashAction();
    public static SplashAction OnSplashFinished;

    public Animator animPage1, animPage2;
    public GameObject menuPanel, canvasSplash, introBackground;

    [Space]
    [Range(1f, 10f)]
    public float delayPages = 2f;

    private void Awake()
    {
       // menuPanel.SetActive(false);
        canvasSplash.SetActive(true);

        Screen.orientation = ScreenOrientation.Landscape;
    }

    private void OnEnable()
    {
        //canvasSplash.SetActive(true);
        introBackground.SetActive(true);
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {

        animPage1.SetBool("show", true);
        yield return new WaitForSeconds(0.7f);//wait to fully loaded
        yield return new WaitForSeconds(delayPages);
        animPage1.SetBool("show", false);//hide page
        yield return new WaitForSeconds(0.7f);//wait to fully unloaded
        animPage2.SetBool("show", true);
        //wait for page 2 view
        //animPage2.SetBool("show", true);
        yield return new WaitForSeconds(0.7f);//wait to fully unloaded
        yield return new WaitForSeconds(delayPages+1f);
        introBackground.SetActive(false);
        //menuPanel.SetActive(true);
        animPage2.SetBool("show", false);
        OnSplashFinished?.Invoke();

        Screen.orientation = ScreenOrientation.AutoRotation;

        yield return new WaitForSeconds(0.7f);//wait to fully unloaded
        //yield return new WaitForSeconds(0.7f);//wait to fully unloaded
        
        canvasSplash.SetActive(false);//close all
        yield break;
    }

}
