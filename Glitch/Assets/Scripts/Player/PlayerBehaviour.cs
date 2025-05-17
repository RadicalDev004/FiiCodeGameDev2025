using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float interactionRange = 5f;
    public Camera playerCamera;
    private Editable editableToAccess = null;
    private UI UI;
    private MapManager mapManager;
    public bool isEpressed = false;
    public Animator StaffAnimator;
    [Header("Health")]
    public float MaxHealth, InitialMaxHealth;
    public Slider S_HealthSlider;
    public float CurrentHealth;
    private Coroutine Heal;
    public float ScaleInitialHealthSlider;
    public static bool isDead = false;

    [Header("Projectiles")]
    public Projectile OrgProjectile;
    public float Lifetime = 3, Size = 1, Speed = 0.5f, Damage = 15;
    public float CooldownTimer = 1;
    private bool isCoolDown = false;
    public static bool EnabledProjectiles = true;
    public MeshRenderer StaffStone;
    public static int GlitchesSolved;
    public static float ProjectileSizeIncrease;

    [Header("Other")]
    public List<ParticleSystem> GlitchSolve = new();

    private float lastSeenEditableTime = 0f;
    [SerializeField] private float uiHideDelay = 0.3f;
    public GameObject MagicStaff;
    public Transform StaffAnimation;
    public TMP_InputField inputField_questions;

    private void Awake()
    {
        GlitchesSolved = 0;
        InitialMaxHealth = MaxHealth;
        UI = Ref.UI;
        mapManager = UI.gameObject.GetComponent<MapManager>();

        CurrentHealth = MaxHealth;
        S_HealthSlider.maxValue = MaxHealth;
        S_HealthSlider.value = CurrentHealth;
        ScaleInitialHealthSlider = S_HealthSlider.GetComponent<RectTransform>().sizeDelta.x / 4;
    }

    void Update()
    {
        //Debug.Log(GlitchesSolved);
        //Debug.Log(CurrentHealth.ToString()); 
        CheckForEditableObject();

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    UI.ToggleAdmin();
        //}

        if (Input.GetKeyDown(KeyCode.E) && editableToAccess != null && !Code.IsOpen && !(editableToAccess is Enemy && !ManaSystem.Instance.HasFullMana()) && !MapManager.IsOpen)
        {
            if (!editableToAccess.Block && !Ref.Tutorial.Ongoing)
            {
                //Time.timeScale = 0;
                editableToAccess.OpenTerminal();
                UI.TogglePressE(false);
            }

        }

        if (Input.GetKeyDown(KeyCode.M) && !Code.IsOpen && !MapManager.IsOpen)
        {
            mapManager.ToggleMap(true);
        }

        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0 && !isCoolDown && EnabledProjectiles)
        {
            StartCoroutine(ShootingProjectileCor(0.2f));
        }

        if (Time.time - lastSeenEditableTime > uiHideDelay)
        {
            UI.TogglePressE(false);
            UI.ToggleNotEnughMana(false);
        }

        if (ManaSystem.Instance.HasFullMana() && UI.I_NotEnoughMana.activeInHierarchy)
        {
            UI.TogglePressE(true);
            UI.ToggleNotEnughMana(false);
        }

        if (Input.GetKeyDown(KeyCode.F) && Ref.Tutorial.Ongoing == true && !inputField_questions.isFocused)
        {
            Ref.Tutorial.StopCurrentTutorial();
            UI.TogglePressF(false);
        }
    }

    private void CheckForEditableObject()
    {
        if (Code.IsOpen) return;

        Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);

        int layer = LayerMask.NameToLayer("Projectile");
        int layerMask = ~(1 << layer);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, layerMask))
        {
            if (!hit.collider.CompareTag("Editable"))
                return;

            Editable newEditable = hit.collider.GetComponent<Editable>() ?? hit.collider.transform.parent.GetComponent<Editable>();
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
                if (editableToAccess is Enemy)
                {
                    UI.TogglePressE(ManaSystem.Instance.HasFullMana());
                    UI.ToggleNotEnughMana(!ManaSystem.Instance.HasFullMana());
                }
                else
                {
                    if (!Ref.Tutorial.GhostAnimator.gameObject.activeInHierarchy)
                        UI.TogglePressE(true);
                    else
                        UI.TogglePressE(false);
                }
                return;
            }
        }

        if (editableToAccess != null && Time.time - lastSeenEditableTime > uiHideDelay)
        {
            editableToAccess.ToggleOutline(false);
            editableToAccess = null;
        }
    }

    public void PlaySolveGlitch()
    {
        for (int i = 0; i < GlitchSolve.Count; i++)
        {
            GlitchSolve[i].Play();
        }
    }

    public void ShootProjectile()
    {

    }

    private IEnumerator ShootingProjectileCor(float animTime)
    {
        AudioManager.Play("Projectile_Cast");
        StaffAnimator.SetTrigger("shoot");
        isCoolDown = true;
        Vector3 initialPos = MagicStaff.transform.localPosition;
        Quaternion initialRot = MagicStaff.transform.localRotation;
        //Tween.LocalPosition(MagicStaff.transform, StaffAnimation.localPosition, animTime, 0, Tween.EaseInOut);
        //Tween.LocalRotation(MagicStaff.transform, StaffAnimation.localRotation, animTime, 0, Tween.EaseInOut);
        yield return new WaitForSeconds(animTime);

        Projectile pj = Instantiate(OrgProjectile, OrgProjectile.transform.parent);
        pj.gameObject.SetActive(true);
        pj.Shoot(Lifetime, Speed, Size + ProjectileSizeIncrease * Size / 15, Damage);
        pj.transform.SetParent(null);


        //Tween.LocalPosition(MagicStaff.transform, initialPos, animTime, 0, Tween.EaseInOut);
        //Tween.LocalRotation(MagicStaff.transform, initialRot, animTime, 0, Tween.EaseInOut);


        yield return new WaitForSeconds(CooldownTimer - animTime);
        isCoolDown = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        CurrentHealth -= amount;
        if(CurrentHealth < 0)  CurrentHealth = 0;
        var a = Tween.Value(S_HealthSlider.value, CurrentHealth, val => S_HealthSlider.value = val, 0.5f, 0, Tween.EaseInOut);

        if (CurrentHealth == 0)
        {
            isDead = true;
            //Tween.Stop(a.targetInstanceID);
            Ref.ActionAfterTime(0.5f, delegate
            {
                Ref.SaveSystem.LoadState();
            });
            
            return;
        }
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        if(amount > 0)
        {
            Ref.UI.OnHit();
        }

        if(amount < 0)
        {
            if(Heal != null) StopCoroutine(Heal);
            Heal = StartCoroutine(HealCor());
        }
    }


    private IEnumerator HealCor()
    {
        yield return new WaitForSeconds(20);
        while(CurrentHealth < MaxHealth)
        {
            CurrentHealth += 0.25f;
            S_HealthSlider.value = CurrentHealth;
            yield return new WaitForSeconds(1);
        }
    }

    public Editable GetEditable()
    {
        if(editableToAccess != null)
            return editableToAccess;
        else
            return null;
    }

    public void IncreaseMaxHealth()
    {
        MaxHealth += 50;
        S_HealthSlider.maxValue = MaxHealth;
        S_HealthSlider.value = MaxHealth;
        CurrentHealth = MaxHealth;

        S_HealthSlider.GetComponent<RectTransform>().sizeDelta += new Vector2(ScaleInitialHealthSlider, 0f);
    }    

    public void ResetHealth(float curr)
    {
        if (Heal != null) StopCoroutine(Heal);
        S_HealthSlider.GetComponent<RectTransform>().sizeDelta = new(4 * ScaleInitialHealthSlider, S_HealthSlider.GetComponent<RectTransform>().sizeDelta.y);
        MaxHealth = InitialMaxHealth;
        S_HealthSlider.maxValue = MaxHealth;
        S_HealthSlider.value = curr;
        Debug.LogError("Resetting health " + curr + " " + S_HealthSlider.value);
        CurrentHealth = curr;
        isDead = false;
    }
}