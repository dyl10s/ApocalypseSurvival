using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour

{

    public Slider healthBar;
    Text healthText;


    PlayerController playerController;
    BaseWeapon baseWeapon;

    public void SendDamage(int dam)
    {
        PlayerController playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerStats.TakeDamage(dam);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        HealthBar health = GameObject.FindGameObjectWithTag("Health").GetComponent<HealthBar>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        baseWeapon = GameObject.Find("ItemDrop").GetComponent<BaseWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerController.health;
        healthText.text = healthBar.value.ToString();
        if(healthBar.value <= 25) {
            healthText.color = Color.red;
        }
        else {
            healthText.color= Color.white;
        }
    }

    
}
