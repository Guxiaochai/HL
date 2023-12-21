using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Log : MonoBehaviour
{

    public static string CurrentScenePath => SceneManager.GetActiveScene().path;

    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(CurrentScenePath);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
