using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//changes Scenes when Button is pressed
public class ChangeScene : MonoBehaviour
{
    public void SwitchScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    // closes application when Quit Button is pressed
    public void Quit()
    {
        Application.Quit();
    }

}
