using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObbExctractor : MonoBehaviour
{
    public void LoadEasy() { SceneManager.LoadScene("#Easy"); }
    public void LoadVuforia() { SceneManager.LoadScene("#Vuforia"); }
    public void LoadMenu() { SceneManager.LoadScene("Splash"); }
    public void ExitApp() {
        Debug.Log("ante geia!");
        Application.Quit(); 
    }
    
}
