using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour

{

    public Slider healthBar;
    Text healthText;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        HealthBar health = GameObject.FindGameObjectWithTag("Health").GetComponent<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerController.Health;
    }

    
}
