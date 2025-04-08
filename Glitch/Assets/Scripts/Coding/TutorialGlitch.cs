using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialGlitch : Editable
{
    public List<Texture> GlitchSprites = new();
    public Material GlitchMaterial;
    public List<string> CorrectAnswer = new();

    private void Awake()
    {
        StartCoroutine(Animation());
        ValidateCode = Validate;
        GlitchMaterial = GetComponent<MeshRenderer>().material;
    }

    public bool Validate(List<string> code)
    {
        
        if (code.Count != CorrectAnswer.Count)
        {
            Debug.LogError("Failed validation at length " + code.Count);
            return false;
        }

        if (code[0] != "true" && code[0] != "false")
        {
            Debug.LogError("Failed validation at type " + code[0]);
            return false;
        }

        if(code.SequenceEqual(CorrectAnswer))
        {
            OnGlitchSolve();
        }
       

        return true;
    }

    public IEnumerator Animation()
    {       
        while (true)
        {
            for(int i = 0; i < GlitchSprites.Count; i++)
            {
                if (Completed)
                {
                    GetComponent<MeshRenderer>().enabled = false;
                    yield break;
                }
                GlitchMaterial.SetTexture("_BaseMap", GlitchSprites[i]);
                yield return new WaitForSeconds(0.2f);
            }
            
        }
    }
}
