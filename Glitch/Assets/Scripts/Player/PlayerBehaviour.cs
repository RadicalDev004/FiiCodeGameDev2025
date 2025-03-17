using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector]
    public List<Editable> EditablesInRange;
    public Editable EditableToAcess = null;
    private UI UI;
    public bool isEpressed = false;
    [Header("Projectiles")]
    public Projectile OrgProjectile;
    public float Lifetime = 3, Size = 1, Speed = 0.5f;
    public float CooldownTimer = 1;
    private bool isCoolDown = false;

    public List<ParticleSystem> GlithcSolve = new();

    private void Awake()
    {
        UI = Ref.UI;

        ToggleSolveGlitch(false);
    }



    void Update()
    {
        if(!Code.IsOpen)
        {
            UI.TogglePressE(EditablesInRange.Count > 0);
        }
        if(EditablesInRange.Count > 0)
        {
            EditableToAcess = GetClosestEditableInRange();
            EditableToAcess.ToggleOutline(true);
            ToggleOffOtherEditablesInRange(EditableToAcess);
        }
        if(Input.GetKeyDown(KeyCode.E)) 
        {            
            if (EditableToAcess != null && !EditableToAcess.Completed && !Code.IsOpen)
            {
                Time.timeScale = 0;
                EditableToAcess.CreateTerminal();
                UI.TogglePressE(false);
            }

        }
        if(Input.GetMouseButtonDown(0) && Time.timeScale > 0 && !isCoolDown)
        {
            ShootProjectile();
            StartCoroutine(ShootingCooldwon());
        }
    }

    public void ToggleSolveGlitch(bool state)
    {
        for(int i = 0; i < GlithcSolve.Count; i++)
        {
            if(state)
                GlithcSolve[i].Play();
            else
                GlithcSolve[i].Stop();
        }
        if (!state) return;
        StartCoroutine(WaitAndStopAnimation());
        
    }
    IEnumerator WaitAndStopAnimation()
    {
        yield return new WaitForSecondsRealtime(5);
        ToggleSolveGlitch(false);
    }

    public void ShootProjectile()
    {
        Projectile pj = Instantiate(OrgProjectile, OrgProjectile.transform.parent);
        pj.gameObject.SetActive(true);
        pj.Shoot(Lifetime, Speed, Size);
        pj.transform.SetParent(null);
    }

    private IEnumerator ShootingCooldwon()
    {
        isCoolDown = true;
        yield return new WaitForSeconds(CooldownTimer);
        isCoolDown = false;
    }

    public Editable GetClosestEditableInRange()
    {
        float mindist = 1000;
        Editable min = null;
        foreach(var ed in EditablesInRange)
        {
            if(ed != null && Vector3.Distance(ed.gameObject.transform.position, transform.position) < mindist)
            {
                min = ed;
                mindist = Vector3.Distance(ed.gameObject.transform.position, transform.position);
            }
            if(ed == null)
            {
                EditablesInRange.Remove(ed);
            }
        }
        return min;
    }

    public void ToggleOffOtherEditablesInRange(Editable Current)
    {
        foreach(var ed in EditablesInRange)
        {
            if(ed != Current)
            {
                ed.ToggleOutline(false);
            }
        }
    }

    public void AddEditable(Editable Edt)
    {
        if (EditablesInRange.Contains(Edt))
            return;
        EditablesInRange.Add(Edt);
    }

    public void RemoveEditable(Editable Edt)
    {
        if (!EditablesInRange.Contains(Edt))
            return;
        EditablesInRange.Remove(Edt);
    }
}
