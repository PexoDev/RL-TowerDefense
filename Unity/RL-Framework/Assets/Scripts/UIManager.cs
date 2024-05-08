using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text goldText;
    public Text healthText;
    public Text waveText;

    public EconomyManager economyManager;

    void Update()
    {
        goldText.text = "Gold: " + economyManager.Gold.ToString();
        // Update health and wave information similarly
    }
}