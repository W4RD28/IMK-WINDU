using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitAR : MonoBehaviour
{
    public Canvas start;
    public Button exit;

    public void Exit()
    {
        Application.Quit();
    }
}
