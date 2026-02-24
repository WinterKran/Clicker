using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ClickerGame : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;

    public GameObject floatingTextPrefab;
    public Transform canvasTransform;

    [Header("Click System")]
    public int clickPower = 1;
    public int clickUpgradeCost = 20;
    public Button clickUpgradeButton;
    public TextMeshProUGUI clickPowerText;
    public TextMeshProUGUI clickUpgradeCostText;

    [Header("Auto System")]
    public int autoPower = 1;
    public float autoInterval = 1f;
    public TextMeshProUGUI autoPowerText;

    [Header("Upgrade Settings")]
    public float costMultiplier = 1.15f;

    [Header("Auto Upgrade")]
    public int autoUpgradeCost = 50;
    public Button autoUpgradeButton;
    public TextMeshProUGUI autoUpgradeCostText;

    [Header("Click Upgrade Duplicate")]
    public int clickUpgradeCost2 = 100;
    public Button clickUpgradeButton2;
    public TextMeshProUGUI clickUpgradeCostText2;
    public TextMeshProUGUI clickUpgradePowerText2;

    void Start()
{
    score = PlayerPrefs.GetInt("score", 0);

    clickPower = PlayerPrefs.GetInt("clickPower", 1);
    clickUpgradeCost = PlayerPrefs.GetInt("clickUpgradeCost", 20);

    autoPower = PlayerPrefs.GetInt("autoPower", 1);
    autoUpgradeCost = PlayerPrefs.GetInt("autoUpgradeCost", 50);

    clickUpgradeCost2 = PlayerPrefs.GetInt("clickUpgradeCost2", 100);
    

    InvokeRepeating("AutoClick", autoInterval, autoInterval);

    UpdateUI();
    PlayerPrefs.Save();
}

    public void AddScore()
{
    score += clickPower;
    Vector3 mousePos = Mouse.current.position.ReadValue();
    ShowFloatingText("+" + clickPower, Color.yellow, mousePos);

    SaveAndUpdate();
}
    void AutoClick()
{
    score += autoPower;

    Vector3 pos = scoreText.transform.position;
    pos.x -= Random.Range(50f, 150f);
    pos.y += Random.Range(-70f, -80f);

ShowFloatingText("+" + autoPower, Color.cyan, pos);

    ShowFloatingText("+" + autoPower, Color.cyan, pos);

    SaveAndUpdate();
}

    public void BuyClickUpgrade()
{
    if (score >= clickUpgradeCost)
    {
        score -= clickUpgradeCost;
        clickPower++;

        ShowFloatingText("Click Power +1", Color.green, clickUpgradeButton.transform.position);

        clickUpgradeCost = Mathf.RoundToInt(clickUpgradeCost * costMultiplier);
        SaveAndUpdate();
    }
}

    void SaveAndUpdate()
{
    PlayerPrefs.SetInt("score", score);
    PlayerPrefs.SetInt("clickPower", clickPower);
    PlayerPrefs.SetInt("clickUpgradeCost", clickUpgradeCost);
    PlayerPrefs.SetInt("autoPower", autoPower);
    PlayerPrefs.SetInt("autoUpgradeCost", autoUpgradeCost);
    PlayerPrefs.SetInt("clickUpgradeCost2", clickUpgradeCost2);

    PlayerPrefs.Save();   

    UpdateUI();
}

    void UpdateUI()
{
    scoreText.text = "Click: " + score;

    clickPowerText.text = "Click Power: " + clickPower;
    clickUpgradeCostText.text = "Cost: " + clickUpgradeCost;

    autoPowerText.text = "Auto Power: " + autoPower;
    autoUpgradeCostText.text = "Auto Cost: " + autoUpgradeCost;

    clickUpgradeCostText2.text = "Cost 2: " + clickUpgradeCost2;
    clickUpgradePowerText2.text = "Power After x2: " + (clickPower * 2);

    UpdateClickButtonState();
    UpdateClickButtonState2();
    UpdateAutoButtonState();
}

    void UpdateClickButtonState()
    {
        if (score >= clickUpgradeCost)
        {
            clickUpgradeButton.interactable = true;
            clickUpgradeButton.image.color = Color.green;
        }
        else
        {
            clickUpgradeButton.interactable = false;
            clickUpgradeButton.image.color = Color.red;
        }
    }

    public void BuyAutoUpgrade()
{
    if (score >= autoUpgradeCost)
    {
        score -= autoUpgradeCost;
        autoPower++;

        ShowFloatingText("Auto Power +1", Color.green, autoUpgradeButton.transform.position);

        autoUpgradeCost = Mathf.RoundToInt(autoUpgradeCost * costMultiplier);
        SaveAndUpdate();
    }
}

void UpdateAutoButtonState()
{
    if (score >= autoUpgradeCost)
    {
        autoUpgradeButton.interactable = true;
        autoUpgradeButton.image.color = Color.green;
    }
    else
    {
        autoUpgradeButton.interactable = false;
        autoUpgradeButton.image.color = Color.red;
    }
}

public void BuyClickUpgrade2()
{
    if (score >= clickUpgradeCost2)
    {
        score -= clickUpgradeCost2;

        clickPower *= 2;

        ShowFloatingText("CLICK POWER x2!", Color.magenta, clickUpgradeButton2.transform.position);

        clickUpgradeCost2 *= 10;

        SaveAndUpdate();
    }
}

void UpdateClickButtonState2()
{
    if (score >= clickUpgradeCost2)
    {
        clickUpgradeButton2.interactable = true;
        clickUpgradeButton2.image.color = Color.green;
    }
    else
    {
        clickUpgradeButton2.interactable = false;
        clickUpgradeButton2.image.color = Color.red;
    }


}

public void ResetSave()
{
    PlayerPrefs.DeleteAll();   // ลบทุกค่า
    PlayerPrefs.Save();

    // รีเซ็ตค่ากลับค่าเริ่มต้น
    score = 0;
    clickPower = 1;
    clickUpgradeCost = 20;
    autoPower = 1;
    autoUpgradeCost = 50;
    clickUpgradeCost2 = 100;

    UpdateUI();
}

void ShowFloatingText(string message, Color color, Vector3 screenPosition)
{
    GameObject ft = Instantiate(floatingTextPrefab, canvasTransform);

    RectTransform canvasRect = canvasTransform as RectTransform;
    RectTransform ftRect = ft.GetComponent<RectTransform>();

    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRect,
        screenPosition,
        null,
        out localPoint
    );

    ftRect.localPosition = localPoint;

    FloatingText floating = ft.GetComponent<FloatingText>();
    floating.SetText(message);

    TextMeshProUGUI text = ft.GetComponent<TextMeshProUGUI>();
    text.color = color;
}



}