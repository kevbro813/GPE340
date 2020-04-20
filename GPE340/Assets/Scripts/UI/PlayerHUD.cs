using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Text critChanceText; // Text that will update to show crit chance value
    public Text critDamageText; // Text that will update to show crit damage value
    public Image healthBar; // Health bar image
    // Images representing one life each
    public Image lifeOne;
    public Image lifeTwo;
    public Image lifeThree;
    // Image for the weapon icon
    private Image weaponImage;
    public GameObject weaponImageObject; // weapon icon game object used to SetActive
    public Image enemyHealthBarPrefab;
    public Transform enemyHealthContainer;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.type = Image.Type.Filled; // Set the health bar image type to filled
        weaponImage = weaponImageObject.GetComponent<Image>(); // Get the weapon icon's image component
        SetWeaponIcon(); // Set initial icon
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

    // Update the number of lives displayed (opaque vs translucent)
    public void UpdateLives()
    {
        if (GameManager.instance.playerLives == 3) // Show three lives
        {
            SetIconVisible(lifeOne);
            SetIconVisible(lifeTwo);
            SetIconVisible(lifeThree);
        }
        else if (GameManager.instance.playerLives == 2) // Show two lives
        {
            SetIconVisible(lifeOne);
            SetIconVisible(lifeTwo);
            SetIconTranslucent(lifeThree);
        }
        else if (GameManager.instance.playerLives == 1) // Show one life
        {
            SetIconVisible(lifeOne);
            SetIconTranslucent(lifeTwo);
            SetIconTranslucent(lifeThree);
        }
        else if (GameManager.instance.playerLives == 0) // Show no lives
        {
            SetIconTranslucent(lifeOne);
            SetIconTranslucent(lifeTwo);
            SetIconTranslucent(lifeThree);
        }
        else
        {
            // Invalid Life count
            Debug.Log("Error: Invalid quantity of lives");
        }
    }
    // Set an image to opaque
    private void SetIconVisible(Image image) // Pass in the image
    {
        Color color = image.color; // Get the image color
        color.a = 1.0f; // Set to opaque
        image.color = color; // Reset image color
    }
    // Set an image to transluscent
    private void SetIconTranslucent(Image image) // Pass in the image
    {
        Color color = image.color; // Get the image color
        color.a = 0.3f; // Set to translucent
        image.color = color;// Reset image color
    }

    // Set the weapon icon on the HUD
    public void SetWeaponIcon()
    {
        if (GameManager.instance.currentWeaponIcon) // Check if there is a currentWeaponIcon
        {
            weaponImageObject.SetActive(true); // Set the icon game object to active
            weaponImage.sprite = GameManager.instance.currentWeaponIcon; // Set the weapon icon image sprite to the currentWeaponIcon
        }
        else if (!GameManager.instance.currentWeaponIcon) // If currentWeaponIcon is null, then the player does not have a weapon
        {
            weaponImageObject.SetActive(false); // Set the weapon icon game object to inactive
        }
    }
}
