using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Editable
{
    public Dictionary<string, int> Functions = new();
    public Dictionary<string, int> LegendaryFunctions = new();
    public Image I_PowerUpOrg;
    public List<Sprite> PowerupSprites = new();
    

    public List<(string, Image)> OtherSprites = new();

    private void Awake()
    {
        ValidateCode = Validate;
    }

    void Update()
    {
        if(!Ref.Tutorial.Ongoing && !Settings.IsOpen && !MapManager.IsOpen && !Code.IsOpen && Input.GetKeyDown(KeyCode.Q)) 
        {
            CreateinventoryCode();
            StartCoroutine(ToggleTerminal(true));
        }
    }

    public void CollectFunction(string functionName)
    {
        if(Functions.ContainsKey(functionName))
        {
            Functions[functionName]++;
        }
        else
        {
            Functions.Add(functionName, 1);
        }
        
    }

    public void CollectLegendaryFunction(string functionName)
    {
        if (LegendaryFunctions.ContainsKey(functionName))
        {
            LegendaryFunctions[functionName]++;
        }
        else
        {
            LegendaryFunctions.Add(functionName, 1);
        }

    }

    public bool Validate(List<string> code)
    {
        if(code.Count != 1)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }
        if (!FunctionItem.functionNames.Contains(code[0]) && !FunctionItem.legendaryFunctions.Contains(code[0]))
        {
            Debug.LogError("Failed validation at type " + code[0]);
            return false;
        }

        if (code[0] == FunctionItem.functionNames[0])
        {
            StartCoroutine(ShowPowerUpUI(Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent), PowerupSprites[0], 1));
            Ref.PlayerBehaviour.TakeDamage(-50);
        }
        else if (code[0] == FunctionItem.functionNames[1])
        {
            StartCoroutine(ShowPowerUpUI(Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent), PowerupSprites[1], 60));
            StartCoroutine(AttackBoost(60));
        }
        else if (code[0] == FunctionItem.functionNames[2])
        {
            StartCoroutine(ShowPowerUpUI(Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent), PowerupSprites[2], 60));
            StartCoroutine(SpeedIncrease(60));
        }
        else if (code[0] == FunctionItem.functionNames[3])
        {
            StartCoroutine(ShowPowerUpUI(Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent), PowerupSprites[3], 60));
            StartCoroutine(AttackSpeedIncrease(60));
        }
        else if (code[0] == FunctionItem.functionNames[4])
        {
            StartCoroutine(ShowPowerUpUI(Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent), PowerupSprites[4], 60));
            StartCoroutine(ProjectilePassThrough(60));
        }

        if (code[0] == FunctionItem.legendaryFunctions[0])
        {
            Ref.PlayerBehaviour.IncreaseMaxHealth();
        }
        else if (code[0] == FunctionItem.legendaryFunctions[1])
        {
            PlayerBehaviour.ProjectileSizeIncrease++;
        }
        else if (code[0] == FunctionItem.legendaryFunctions[2])
        {
            ManaSystem.ExtraManaPerHit++;
        }
        if (Functions.ContainsKey(code[0])) Functions[code[0]]--;
        if (LegendaryFunctions.ContainsKey(code[0])) LegendaryFunctions[code[0]]--;

        return true;
    }

    public void CreateinventoryCode()
    {
        string fnc = string.Join("", Functions.Select(kvp => kvp.Value > 0 ? $"  {kvp.Key} x{kvp.Value}\r\n" : ""));
        string legendaryFnc = $"<color=orange>{string.Join("", LegendaryFunctions.Select(kvp => kvp.Value > 0 ? $"  {kvp.Key} x{kvp.Value}\r\n" : ""))}</color>";
        ExecutableCode = "\r\n<color=#c2c2c2>/* you have:\r\n" +
            $"{fnc}" +
            $"\r\n{legendaryFnc}"+
            "\r\n*/</color>\r\n\r\n" +
            "use_powerup(<e></e> );\r\n\r\n" +
            "<color=#44cd8b>/* use collected functions to help you during battle */</color>\r\n";
    }

    private IEnumerator ShowPowerUpUI(Image img, Sprite sprt, float timer = -1)
    {
        img.sprite = sprt;
        img.SetNativeSize();
        float mx = Mathf.Max(img.GetComponent<RectTransform>().sizeDelta.x, img.GetComponent<RectTransform>().sizeDelta.y);
        float imp = mx / 75;
        img.GetComponent<RectTransform>().sizeDelta = img.GetComponent<RectTransform>().sizeDelta / imp;
        img.gameObject.SetActive(true);

        if (timer == -1) yield break;

        img.fillAmount = 1;

        float elapsedTime = 0f;
        float startValue = img.fillAmount;

        while (elapsedTime < timer)
        {
            elapsedTime += Time.deltaTime;
            img.fillAmount = Mathf.Lerp(startValue, 0, elapsedTime / timer);
            yield return null;
        }

        img.fillAmount = 0;
        Destroy(img.gameObject);
    }

    private IEnumerator AttackBoost(float duration)
    {
        Ref.PlayerBehaviour.Damage *= 2;
        yield return new WaitForSeconds(duration);
        Ref.PlayerBehaviour.Damage /= 2;
    }
    private IEnumerator SpeedIncrease(float duration)
    {
        Ref.Movement.speed *= 2;
        yield return new WaitForSeconds(duration);
        Ref.Movement.speed /= 2;
    }

    private IEnumerator AttackSpeedIncrease(float duration)
    {
        Ref.PlayerBehaviour.CooldownTimer /= 2;
        yield return new WaitForSeconds(duration);
        Ref.PlayerBehaviour.CooldownTimer *= 2;
    }

    private IEnumerator ProjectilePassThrough(float duration)
    {
        Projectile.PassThrough = true;
        yield return new WaitForSeconds(duration);
        Projectile.PassThrough = false;
    }

    public void AddCustomIcon(string name, Sprite sprt)
    {       
        Image img = Instantiate(I_PowerUpOrg, I_PowerUpOrg.transform.parent);
        OtherSprites.Add((name, img));
        StartCoroutine(ShowPowerUpUI(img, sprt));
    }

    public void RemoveCustomIcon(string name)
    {
        foreach(var ci in OtherSprites )
        {
            if (ci.Item1 == name)
            {
                Destroy(ci.Item2.gameObject);
                OtherSprites.Remove(ci);
                break;
            }
        }
    }
}
