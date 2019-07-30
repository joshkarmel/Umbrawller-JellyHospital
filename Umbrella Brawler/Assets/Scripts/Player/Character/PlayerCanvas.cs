using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;


public class PlayerCanvas : MonoBehaviour
{
    //------------------------------------------------------------------------CONSTANTS:

    private const string LOG_TAG = "PlayerCanvas";
    public bool VERBOSE = false;
    //---------------------------------------------------------------------------FIELDS:
    [System.NonSerialized]
    public bool StartTimer;
    [System.NonSerialized]
    public float Timer;
    [System.NonSerialized]
    public Text ScoreText;
    [System.NonSerialized]
    public Image EndState;

    private float tempTimer = 0;


    private PlayerController pC;
    private PlayerInformation pI;

    [SerializeField]
    private Text timeText, timerText, playerHPText, currentAmmoText,
                 playerMaxHPText, maxAmmoText, ultChargeText;

    [SerializeField]
    private Slider playerHPSlider, umbrellaHPSlider, shotgunChargeSlider;

    [SerializeField]
    private Image playerHPFill, umbrellaHPFill, ultFill;
    public Sprite winMessage, loseMessage;

    [SerializeField]
    private Color PLAYER_HP_START_RGB, PLAYER_HP_MID_RGB, UMBRELLA_HP_START_RGB, ULT_METER_START_RGB;
    //---------------------------------------------------------------------MONO METHODS:
    void Start()
    {
        timeText = transform.Find("Timer").GetComponent<Text>();
        timerText = transform.Find("Timer Text").GetComponent<Text>();
        ScoreText = transform.Find("Team Score/Kills").GetComponent<Text>();
        EndState = transform.Find("End State").GetComponent<Image>();

        // Find the Player
        pC = transform.parent.GetComponent<PlayerController>();
        // Find the player information
        pI = transform.parent.GetComponent<PlayerInformation>();

        // Add event handlers to update the HUD 
        if (pC != null)
        {
            // PlayerController and UmbrellaBase raise events when HP changed
            // The right operands here are functions defined in PlayerCanvas that
            // handle the events when they are raised
            pI.OnHPChanged += PCOnHPChanged;

            if (pC.Umbrella != null)
                pC.Umbrella.OnHPChanged += UmbrellaOnHPChanged;

            if (pC.PlayerWeapon != null)
            {
                pC.PlayerWeapon.OnAmmoChanged += WeaponOnAmmoChanged;

                if(pC.PlayerWeapon.ChargeRecoil)
                {
                    pC.PlayerWeapon.OnChargeChanged += WeaponOnChargeChanged;
                }
                else
                {
                    // Hide the slider if the weapon doesn't charge
                    shotgunChargeSlider.gameObject.SetActive(false);
                }
            }

            if (pC.Ultimate != null)
                pC.Ultimate.OnChargeChanged += UltOnChargeChanged;
        }

        // Update UI Based on initial values
        PCOnHPChanged();
        UmbrellaOnHPChanged();
        WeaponOnAmmoChanged();
        WeaponOnChargeChanged();
        UltOnChargeChanged(0); // Ult Charge starts off as 0
    }



    void Update()
    {
        if (StartTimer)
        {
            StartCountdown();
        }
    }
    //--------------------------------------------------------------------------METHODS:
    /// <Summary>
    /// Reset values of timer
    /// </Summary>
    public void ResetVals()
    {
        tempTimer = Timer;
        timeText.text = Timer.ToString();
        timeText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        StartTimer = false;
    }

    /// <Summary>
    /// if not the ground boundary, then activate the canvas
    /// </Summary>
    public void InitializeTimer(string text)
    {
        timeText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        tempTimer = Timer;
        timerText.text = text;
        timeText.text = tempTimer.ToString();
        StartTimer = true;
    }

    /// <summary>
    /// Event handler for Player HP changed, updates HUD
    /// This is assigned as a delegate to Player HP change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void PCOnHPChanged()
    {
        if (pC != null)
        {
            playerHPText.text = pI.PlayerHealth.ToString("f0");
            playerMaxHPText.text = pI.PlayerMaxHealth.ToString();
            playerHPSlider.value = pI.PlayerHealth / pI.PlayerMaxHealth;
        }
    }

    /// <summary>
    /// Event handler for Umbrella HP changed, updates HUD
    /// This is assigned as a delegate to Umbrella HP change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void UmbrellaOnHPChanged()
    {
        if (pC != null && pC.Umbrella != null)
        {
            umbrellaHPSlider.value = pC.Umbrella.HitPoints / pC.Umbrella.MaxHitPoints;
        }
    }

    /// <summary>
    /// Event handler for Weapon Ammo changed, updates HUD
    /// This is assigned as a delegate to Weapon ammo change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void WeaponOnAmmoChanged()
    {
        if (pC.PlayerWeapon != null)
        {
            currentAmmoText.text = pC.PlayerWeapon.GetCurrentShotsLeftInMag().ToString();
            maxAmmoText.text = pC.PlayerWeapon.ShotsPerMag.ToString();
        }
    }

    /// <summary>
    /// Event handler for Weapon Charge changed, updates HUD
    /// This is assigned as a delegate to Weapon ammo change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void WeaponOnChargeChanged()
    {
        if (pC.PlayerWeapon != null)
        {
            shotgunChargeSlider.value = (pC.PlayerWeapon.CurrentCharge > 1) ? 1 : pC.PlayerWeapon.CurrentCharge;
        }
    }

    /// <summary>
    /// Event handler for Ult Charge changed, updates HUD
    /// This is assigned as a delegate to Ult charge change, so it's automatically called
    /// whenever that happens
    /// </summary>
    /// <param name="value"></param>
    private void UltOnChargeChanged(float value)
    {
        if (pC.Ultimate != null)
        {
            float floatValue = value / 100;
            ultFill.fillAmount = floatValue;
            ultChargeText.text = System.Math.Round(ultFill.fillAmount * 100).ToString() + '%';
            UltMeterColorChange(floatValue);
        }
    }

    

    //--------------------------------------------------------------------------HELPERS:
    /// <Summary>
    /// Start countdown and kill players if timer reaches 0
    /// </Summary>
    private void StartCountdown()
    {
        DLog("Start Countdown");
        //Start timer of Timer -- until 0. x.xx 
        tempTimer -= Time.deltaTime;
        
        if (tempTimer > 0)
            timeText.text = tempTimer.ToString("f2");
        else if (tempTimer <= 0 && !pI.IsDead)
        {
            timeText.text = "0";
            DLog("Kill player");
            pI.IsDead = true;
        }
    }

    /// <summary>
    /// Changes the color of the Player HP Bar on value change
    /// </summary>
    /// <param name="value"></param>
    public void PlayerHPBarColorChange(float value)
    {
        // Makes the calculations easier if the value is 0 to 1 instead of 1 to 0
        float scaledValue = System.Math.Abs(value - 1);
        Color tempColor;


        // Gradually change the rgb based on how much HP is left according to a scale
        // factor
        if (scaledValue < 0.5)
        {
            tempColor = playerHPFill.color;
            tempColor.r = ((PLAYER_HP_START_RGB.r * 255) + ((368) * scaledValue)) / 255;
            tempColor.g = ((PLAYER_HP_START_RGB.g * 255) + ((-30) * scaledValue)) / 255;
            tempColor.b = ((PLAYER_HP_START_RGB.b * 255) + ((-256) * scaledValue)) / 255;
            playerHPFill.color = tempColor;
        }
        else
        {
            // Do the math on a 0 to 0.5 scale 
            scaledValue = scaledValue - 0.5f;

            tempColor = playerHPFill.color;
            tempColor.r = ((PLAYER_HP_MID_RGB.r * 255) + ((70) * scaledValue)) / 255;
            tempColor.g = ((PLAYER_HP_MID_RGB.g * 255) + ((-396) * scaledValue)) / 255;
            tempColor.b = ((PLAYER_HP_MID_RGB.b * 255) + ((126) * scaledValue)) / 255;
            playerHPFill.color = tempColor;
        }
    }

    /// <summary>
    /// Changes the color of the Umbrella HP Bar on value change
    /// </summary>
    /// <param name="value"></param>
    public void UmbrellaHPBarColorChange(float value)
    {
        // Makes the calculations easier if the value is 0 to 1 instead of 1 to 0
        float scaledValue = System.Math.Abs(value - 1);
        Color tempColor;

        tempColor = umbrellaHPFill.color;
        tempColor.r = ((UMBRELLA_HP_START_RGB.r * 255) + (148) * scaledValue) / 255;
        tempColor.g = ((UMBRELLA_HP_START_RGB.g * 255) + (-41) * scaledValue) / 255;
        tempColor.b = ((UMBRELLA_HP_START_RGB.b * 255) + (-78) * scaledValue) / 255;
        umbrellaHPFill.color = tempColor;
    }

    /// <summary>
    /// Darkens the background for this PlayerCanvas when the game has ended.
    /// Should be called from GameModeTracker.cs
    /// </summary>
    public void EndGameFade()
    {
        transform.GetComponent<Animator>().SetTrigger("EndState");
    }

    /// <summary>
    /// Changes the color of the Ult meter on value change
    /// </summary>
    /// <param name="value"></param>
    private void UltMeterColorChange(float value)
    {
        Color tempColor;

        tempColor = ultFill.color;
        tempColor.g = ((ULT_METER_START_RGB.g * 255) + (-107) * value) / 255;
        tempColor.b = ((ULT_METER_START_RGB.b * 255) + (61) * value) / 255;
        ultFill.color = tempColor;
    }


    private void DLog(string message)
    {
        if (VERBOSE) LOG_TAG.TPrint(message);
    }
}