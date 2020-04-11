using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Text critChanceText; // Text that will update to show crit chance value
    public Text critDamageText; // Text that will update to show crit damage value
    public Image healthBar; // Health bar image

    // Start is called before the first frame update
    void Start()
    {
        healthBar.type = Image.Type.Filled; // Set the health bar image type to filled
    }

    // Update all of the HUD elements
    public void UpdateHUD(float critChance, float critDamage, float healthPercentage) // Pass in values to update the HUD with
    {
        UpdateCritDamage(critDamage); // Update critDamage value
        UpdateCritChance(critChance); // Update critChance value
        UpdateHealth(healthPercentage); // Update Health bar based on health percentage
    }
    // Update only health bar
    public void UpdateHealth(float hp)
    {
        healthBar.fillAmount = hp; // Set health bar amount to health percentage
    }
    // Update only crit chance
    public void UpdateCritChance(float cc)
    {
        critChanceText.text = cc.ToString(); // Set critChanceText to crit chance value
    }
    // Update only crit damage
    public void UpdateCritDamage(float cd)
    {
        critDamageText.text = cd.ToString(); // Set critDamageText to crit damage value
    }
}
