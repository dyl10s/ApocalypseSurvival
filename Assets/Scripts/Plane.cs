using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Plane : MonoBehaviour
{
    Canvas ui;

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("Canvas").GetComponent<Canvas>();
        ui.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -111)
        {
            ui.enabled = true;
        }

        if(transform.position.y < -426)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
