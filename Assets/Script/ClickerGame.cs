using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class ClickerGame : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;

    public GameObject floatingTextPrefab;
    public Transform canvasTransform;
    [System.Serializable]

public enum MonsterRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]   // 👈 ต้องอยู่ตรงนี้
public class MonsterData
{
    public string monsterName;
    public double baseHP;
    public Color hpBarColor;
    public Sprite monsterSprite;
    public double regenMultiplier;
    public MonsterRarity rarity;
}




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

    [Header("Monster System")]
    public double monsterHP;
    public double monsterMaxHP;
    public int monsterLevel = 1;

    public TextMeshProUGUI monsterHPText;
    public Slider monsterHPSlider;

    public double baseMonsterHP = 10;
    public double hpGrowth = 1.5f;
    public int rewardPerMonster = 5;

    [Header("Monster Random System")]
    public MonsterData[] monsterList;

    private MonsterData currentMonster;

    [Header("Monster Visual")]
    public Image monsterImage;

    [Header("Monster Regen")]
    public float regenInterval = 0.1f;   // ทุก 0.1 วิ
    public double regenPercent = 0.01;   // 1%

    [Header("Monster Time Limit")]
    public float monsterTimeLimit = 20f;
    private float monsterTimer = 0f;

    [Header("Monster Timer UI")]
    public Slider monsterTimerSlider;
    public TextMeshProUGUI monsterTimerText;   // 👈 เพิ่มตัวนี้

    private List<MonsterData> commonList = new List<MonsterData>();
    private List<MonsterData> rareList = new List<MonsterData>();
    private List<MonsterData> epicList = new List<MonsterData>();
    private List<MonsterData> legendaryList = new List<MonsterData>();

    void Start()
{
    score = PlayerPrefs.GetInt("score", 0);

    clickPower = PlayerPrefs.GetInt("clickPower", 1);
    clickUpgradeCost = PlayerPrefs.GetInt("clickUpgradeCost", 20);

    autoPower = PlayerPrefs.GetInt("autoPower", 1);
    autoUpgradeCost = PlayerPrefs.GetInt("autoUpgradeCost", 50);

    clickUpgradeCost2 = PlayerPrefs.GetInt("clickUpgradeCost2", 100);
    

    InvokeRepeating("AutoClick", autoInterval, autoInterval);
    InvokeRepeating(nameof(RegenerateMonster), regenInterval, regenInterval);

    foreach (var monster in monsterList)
{
    switch (monster.rarity)
    {
        case MonsterRarity.Common:
            commonList.Add(monster);
            break;
        case MonsterRarity.Rare:
            rareList.Add(monster);
            break;
        case MonsterRarity.Epic:
            epicList.Add(monster);
            break;
        case MonsterRarity.Legendary:
            legendaryList.Add(monster);
            break;
    }
}

    UpdateUI();
    SpawnMonster();
    PlayerPrefs.Save();
}

    public void AddScore()
{
    DamageMonster(clickPower);

    Vector3 mousePos = Mouse.current.position.ReadValue();
    ShowFloatingText("-" + FormatNumber(clickPower), Color.yellow, mousePos);
}
    void AutoClick()
{
    DamageMonster(autoPower);

    Vector3 pos = scoreText.transform.position;
    pos.x -= Random.Range(50f, 120f);
    pos.y += Random.Range(-80f, 80f);

    ShowFloatingText("-" + FormatNumber(autoPower), Color.cyan, pos);
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

void Update()
{
    if (monsterHP <= 0) return;

    monsterTimer += Time.deltaTime;

    float timeLeft = monsterTimeLimit - monsterTimer;

    // ป้องกันค่าติดลบ
    if (timeLeft < 0)
        timeLeft = 0;

    // อัปเดต Slider
    if (monsterTimerSlider != null)
        monsterTimerSlider.value = timeLeft;

    // ✅ อัปเดต Timer Text
    if (monsterTimerText != null)
        monsterTimerText.text = timeLeft.ToString("0.0") + "s";

    if (monsterTimer >= monsterTimeLimit)
    {
        ForceSpawnNewMonster();
    }
}

    void UpdateUI()
{
    scoreText.text = "Score: " + FormatNumber(score);

    clickPowerText.text = "Click Power: " + clickPower;
    clickUpgradeCostText.text = "Cost: " + FormatNumber(clickUpgradeCost);

    autoPowerText.text = "Auto Power: " + autoPower;
    autoUpgradeCostText.text = "Auto Cost: " + FormatNumber(autoUpgradeCost);

    clickUpgradeCostText2.text = "Cost 2: " + FormatNumber(clickUpgradeCost2);
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
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    // รีเซ็ตระบบเงิน
    score = 0;

    // รีเซ็ตพลัง
    clickPower = 1;
    autoPower = 1;

    // รีเซ็ตค่าอัปเกรด
    clickUpgradeCost = 20;
    autoUpgradeCost = 50;
    clickUpgradeCost2 = 100;

    // รีเซ็ตมอนสเตอร์
    monsterLevel = 1;
    monsterMaxHP = baseMonsterHP;
    monsterHP = monsterMaxHP;

    SpawnMonster();   // สร้างมอนใหม่
    UpdateUI();       // อัปเดต UI ทั้งหมด
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

string FormatNumber(double value)
{
    string[] suffix = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No" };

    int index = 0;

    while (value >= 1000 && index < suffix.Length - 1)
    {
        value /= 1000;
        index++;
    }

    return value.ToString("0.##") + suffix[index];
}

void SpawnMonster()
{
    float roll = Random.Range(0f, 100f);

    List<MonsterData> selectedList = null;

    if (roll <= 1f && legendaryList.Count > 0)
        selectedList = legendaryList;
    else if (roll <= 4f && epicList.Count > 0)
        selectedList = epicList;
    else if (roll <= 14f && rareList.Count > 0)
        selectedList = rareList;
    else if (commonList.Count > 0)
        selectedList = commonList;

    // 🔥 ถ้ายังว่างอีก ให้ fallback ไปใช้ monsterList ตรง ๆ
    if (selectedList == null || selectedList.Count == 0)
    {
        if (monsterList.Length == 0)
        {
            Debug.LogError("No monsters assigned!");
            return;
        }

        currentMonster = monsterList[Random.Range(0, monsterList.Length)];
    }
    else
    {
        currentMonster = selectedList[Random.Range(0, selectedList.Count)];
    }

    monsterMaxHP = currentMonster.baseHP * Mathf.Pow((float)hpGrowth, monsterLevel - 1);
    monsterHP = monsterMaxHP;

    monsterImage.sprite = currentMonster.monsterSprite;

    monsterTimer = 0f;

    if (monsterTimerSlider != null)
    {
        monsterTimerSlider.maxValue = monsterTimeLimit;
        monsterTimerSlider.value = monsterTimeLimit;
    }

    if (monsterTimerText != null)
    monsterTimerText.text = monsterTimeLimit.ToString("0.0") + "s";
    UpdateMonsterUI();
}


void KillMonster()
{
    double reward = rewardPerMonster * monsterLevel;

    score += (int)reward;

    ShowFloatingText("+" + FormatNumber(reward), 
        Color.green, 
        monsterHPText.transform.position);

    monsterLevel++;

    // ✅ รีเซ็ต Timer
    monsterTimer = 0f;

    if (monsterTimerSlider != null)
    {
        monsterTimerSlider.maxValue = monsterTimeLimit;
        monsterTimerSlider.value = monsterTimeLimit;
    }

    SpawnMonster();

    SaveAndUpdate();
}

void DamageMonster(double damage)
{
    // ลดเลือด
    monsterHP -= damage;

    // เอฟเฟกต์โดนตี (ย่อ-ขยาย)
    monsterImage.transform.localScale = Vector3.one * 0.9f;
    Invoke(nameof(ResetScale), 0.05f);

    if (monsterHP <= 0)
    {
        KillMonster();
        return;
    }

    UpdateMonsterUI();
}
void ResetScale()
{
    monsterImage.transform.localScale = Vector3.one;
}

void UpdateMonsterUI()
{
    if (monsterHPSlider != null)
    {
        monsterHPSlider.maxValue = (float)monsterMaxHP;
        monsterHPSlider.value = (float)monsterHP;
    }

    if (monsterHPText != null)
    {
        monsterHPText.text = 
            FormatNumber(monsterHP) + " / " + FormatNumber(monsterMaxHP);
    }
}

void RegenerateMonster()
{
    if (monsterHP <= 0) return;

    double healAmount = monsterMaxHP * regenPercent;

    if (currentMonster != null)
        healAmount *= currentMonster.regenMultiplier;

    monsterHP += healAmount;

    if (monsterHP > monsterMaxHP)
        monsterHP = monsterMaxHP;

    UpdateMonsterUI();
}

void ForceSpawnNewMonster()
{
    ShowFloatingText("Monster Escaped!", Color.red, monsterHPText.transform.position);

    SpawnMonster();
}



}






