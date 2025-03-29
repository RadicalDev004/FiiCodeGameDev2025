using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    public List<Editable> AllPuzzles = new();

    public GameObject Arrow;
    private Editable CurrEditable;
    private int Index = 0;
    public static bool Toggle = true;
    

    void Start()
    {
        CurrEditable = AllPuzzles[Index];
    }

    
    void Update()
    {
        if (!Toggle)
        {
            Arrow.SetActive(false);
            return;
        }

        if(!Arrow.activeInHierarchy)
            Arrow.SetActive(true);

        if (CurrEditable.Completed)
        {
            CurrEditable = AllPuzzles[++Index];
            if(CurrEditable == null)
                Toggle = false;
        }

        Arrow.transform.LookAt(CurrEditable.transform);
    }

}
