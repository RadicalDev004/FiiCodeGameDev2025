using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeteoJar : Editable
{
    [Header("Particular")]
    public static List<string> AllWords = new() { "air", "heat", "smoke", "fire", "wind", "tornado", "energy", "electricity", "singularity" };
    public List<string> CurrentWords = new() { "air", "heat" };
    public List<GameObject> AllPartileSystems = new();
    public List<GameObject> KeyElements = new();
    public int FoundItems = 0;

    private void Awake()
    {
        ValidateCode = Validate;
        GenerateCode();
    }

    public bool Validate(List<string> code)
    {
        if (code.Count != 2)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }
        foreach (var cd in code)
        {
            if (!CurrentWords.Contains(cd))
            {
                Debug.LogError("Failed validation at type " + cd);
                return false;
            }
        }

        

        string combine = GetNewWord(code[0], code[1]);
        int ind = ElementStringToInt(combine);
        int ind2 = KeyElementToInt(combine);

        AllPartileSystems.ForEach(elem => elem.SetActive(false));
        if(ind > -1) AllPartileSystems[ind].SetActive(true);  
        
        if(ind2 > -1)
        {
            KeyElements[ind2].SetActive(false);
            FoundItems++;
        }

        CurrentWords.Add(combine);
        GenerateCode();

        if(FoundItems == 4)
        {
            OnGlitchSolve();
        }

        return true;
    }

    public string GetNewWord(string word1, string word2)
    {

        var pair = string.Compare(word1, word2) < 0 ? (word1, word2) : (word2, word1);

        return pair switch
        {
            ("air", "air") => "wind",
            ("air", "heat") => "smoke",
            ("air", "smoke") => "air",
            ("air", "fire") => "energy",
            ("air", "wind") => "wind",
            ("wind", "wind") => "tornado", 
            ("heat", "smoke") => "fire",
            ("heat", _) => "heat",
            ("fire", "fire") => "air",
            ("fire", _) => "fire",
            ("energy", "energy") => "electricity",
            ("energy", _) => "energy",
            ("electricity", "energy") => "singularity",
            ("electricity", _) => "electricity",
            ("singularity", _) => "singularity",
            ("smoke", _) => "smoke",
            _ => "air"
        };
    }

    public int KeyElementToInt(string elem)
    {
        return elem switch
        {
            "fire" => 0,
            "tornado" => 1,
            "electricity" => 2,
            "singularity" => 3,

            _ => -1
        };
    }

    public int ElementStringToInt(string elem)
    {
        return elem switch
        {
            "smoke" => 0,
            "fire" => 1,
            "wind" => 2,
            "tornado" => 3,
            "energy" => 4,
            "electricity" => 5,
            "singularity" => 6,
            _ => -1
        };
    }

    public void GenerateCode()
    {
        ExecutableCode = $"\r\n delcare: {string.Join(", ", CurrentWords)};" +
            "\r\n\r\n combine(<e></e> , <e></e>);" +
            "\r\n\r\n<color=#44cd8b>/* use the combine function to get new elements. */</color>\r\n";
    }
}
