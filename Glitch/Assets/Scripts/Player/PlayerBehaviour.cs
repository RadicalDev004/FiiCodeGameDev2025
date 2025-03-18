using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public float interactionRange = 5f;
    public Camera playerCamera;
    private Editable editableToAccess = null;
    private UI UI;
    public bool isEpressed = false;
    [Header("Projectiles")]
    public Projectile OrgProjectile;
    public float Lifetime = 3, Size = 1, Speed = 0.5f;
    public float CooldownTimer = 1;
    private bool isCoolDown = false;

    public List<ParticleSystem> GlitchSolve = new();

    private float lastSeenEditableTime = 0f;
    private float uiHideDelay = 0.3f;

    private void Awake()
    {
        UI = Ref.UI;
        ToggleSolveGlitch(false);
    }

    void Update()
    {
        CheckForEditableObject();

        if (Input.GetKeyDown(KeyCode.E) && editableToAccess != null && !Code.IsOpen)
        {
            Time.timeScale = 0;
            editableToAccess.CreateTerminal();
            UI.TogglePressE(false);
        }

        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0 && !isCoolDown)
        {
            ShootProjectile();
            StartCoroutine(ShootingCooldown());
        }

        if (Time.time - lastSeenEditableTime > uiHideDelay)
        {
            UI.TogglePressE(false);
        }
    }

    private void CheckForEditableObject()
    {
        if (Code.IsOpen) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.CompareTag("Editable"))
            {
                Editable newEditable = hit.collider.GetComponent<Editable>();
                if (newEditable != null && !newEditable.Completed)
                {
                    if (editableToAccess != newEditable)
                    {
                        if (editableToAccess != null)
                            editableToAccess.ToggleOutline(false);

                        editableToAccess = newEditable;
                        editableToAccess.ToggleOutline(true);
                    }

                    lastSeenEditableTime = Time.time; 
                    UI.TogglePressE(true);
                    return;
                }
            }
        }

        if (editableToAccess != null && Time.time - lastSeenEditableTime > uiHideDelay)
        {
            editableToAccess.ToggleOutline(false);
            editableToAccess = null;
        }
    }

    public void ToggleSolveGlitch(bool state)
    {
        for (int i = 0; i < GlitchSolve.Count; i++)
        {
            if (state)
                GlitchSolve[i].Play();
            else
                GlitchSolve[i].Stop();
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

    private IEnumerator ShootingCooldown()
    {
        isCoolDown = true;
        yield return new WaitForSeconds(CooldownTimer);
        isCoolDown = false;
    }
}