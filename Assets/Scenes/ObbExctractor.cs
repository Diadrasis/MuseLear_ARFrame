using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObbExctractor : MonoBehaviour
{
    public void LoadEasy() { SceneManager.LoadScene("#Easy"); }
    public void LoadVuforia() { SceneManager.LoadScene("#Vuforia"); }
    public void LoadMenu() { SceneManager.LoadScene("Splash"); }
    public void ExitApp() { Application.Quit(); }
   

    void Start()
    {
        //StartCoroutine(ExtractObbDatasets());
        //SceneManager.LoadScene("#Easy");
        ExitApp();
        //test
    }

     /*
    private IEnumerator ExtractObbDatasets()
    {
        string[] filesInOBB = { "arframe.dat", "arframe.xml" };
        foreach (var filename in filesInOBB)
        {
            string uri = Application.streamingAssetsPath + "/Vuforia/" + filename;

            string outputFilePath = Application.persistentDataPath + "/Vuforia/" + filename;
            if (!Directory.Exists(Path.GetDirectoryName(outputFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

            var www = new WWW(uri);
            yield return www;

            Save(www, outputFilePath);
            yield return new WaitForEndOfFrame();
        }

        // When done extracting the datasets, Start Vuforia AR scene
        SceneManager.LoadScene("#Main");
    }

    private void Save(WWW w, string outputPath)
    {
        File.WriteAllBytes(outputPath, w.bytes);

        // Verify that the File has been actually stored
        if (File.Exists(outputPath))
        {
            Debug.Log("File successfully saved at: " + outputPath);
        }
        else
        {
            Debug.Log("Failure!! - File does not exist at: " + outputPath);
        }
    }

    */
}
