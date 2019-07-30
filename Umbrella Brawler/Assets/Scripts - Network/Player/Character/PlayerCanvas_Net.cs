using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PlayerCanvas_Net : NetworkBehaviour
{
    //------------------------------------------------------------------------CONSTANTS:

    private const string LOG_TAG = "PlayerCanvas_Net";
    public bool VERBOSE = false;
    //---------------------------------------------------------------------------FIELDS:
    [System.NonSerialized]
    public bool StartTimer;
    [System.NonSerialized]
    public float Timer;
    [System.NonSerialized]
    public Text ScoreText, EndState;

    private float tempTimer = 0;


    private PlayerController_Net pC;
    private PlayerInformation_Net PInfo;

    [SerializeField]
    private Text timeText, timerText, playerHPText, currentAmmoText,
                 playerMaxHPText, maxAmmoText, ultChargeText;

    [SerializeField]
    private Slider playerHPSlider, umbrellaHPSlider;

    [SerializeField]
    private Image playerHPFill, umbrellaHPFill, ultFill;

    [SerializeField]
    private Color PLAYER_HP_START_RGB, PLAYER_HP_MID_RGB, UMBRELLA_HP_START_RGB, ULT_METER_START_RGB;
    //---------------------------------------------------------------------MONO METHODS:
    void Start()
    {
		if(!isLocalPlayer)
		{
			//Then remove script
            Destroy(transform.Find("PlayerCanvas").gameObject);
			Destroy(this);
			return;
		}
		
        
        timeText = transform.Find("PlayerCanvas").transform.Find("Timer").GetComponent<Text>();
        timerText = transform.Find("PlayerCanvas").transform.Find("Timer Text").GetComponent<Text>();
        ScoreText = transform.Find("PlayerCanvas").transform.Find("Team Score").GetComponent<Text>();
        EndState = transform.Find("PlayerCanvas").transform.Find("End State").GetComponent<Text>();

        // Find the Player
        pC = GetComponent<PlayerController_Net>();
        PInfo = GetComponent<PlayerInformation_Net>();

        PInfo.OnHPChanged += PCOnHPChanged;
        // Add event handlers to update the HUD 
        if (pC != null)
        {
            // PlayerController and UmbrellaBase raise events when HP changed
            // The left operands here are functions defined in PlayerCanvas that
            // handle the events when they are raised
            

            if (pC.UmbrellaNet != null)
                pC.UmbrellaNet.OnHPChanged += UmbrellaOnHPChanged;

            if (pC.PlayerWeaponNet != null)
                pC.PlayerWeaponNet.OnAmmoChanged += WeaponOnAmmoChanged;

            if (pC.Ultimate_Net != null)
                pC.Ultimate_Net.OnChargeChanged += UltOnChargeChanged;
        }
        
        PCOnHPChanged();
        UmbrellaOnHPChanged();
        WeaponOnAmmoChanged();
        UltOnChargeChanged(0); // Ult Charge starts off as 0
    }



    void Update()
    {
        if(!isLocalPlayer)  return;
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
        if(!isLocalPlayer)  return;
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
        if(!isLocalPlayer)  return;
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
        if(!isLocalPlayer)  return;
        if (PInfo != null)
        {
            RpcUpdateHealth();
        }
    }

    /// <summary>
    /// Event handler for Umbrella HP changed, updates HUD
    /// This is assigned as a delegate to Umbrella HP change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void UmbrellaOnHPChanged()
    {
        if(!isLocalPlayer)  return;
        if (pC != null && pC.UmbrellaNet != null)
        {
            umbrellaHPSlider.value = pC.UmbrellaNet.HitPoints / pC.UmbrellaNet.MaxHitPoints;
        }
    }

    /// <summary>
    /// Event handler for Weapon Ammo changed, updates HUD
    /// This is assigned as a delegate to Weapon ammo change, so it's automatically called
    /// whenever that happens
    /// </summary>
    private void WeaponOnAmmoChanged()
    {
        if(!isLocalPlayer)  return;
        if (pC.PlayerWeaponNet != null)
        {
            currentAmmoText.text = pC.PlayerWeaponNet.GetCurrentShotsLeftInMag().ToString();
            maxAmmoText.text = "/" + pC.PlayerWeaponNet.ShotsPerMag.ToString();
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
        if(!isLocalPlayer)  return;
        if (pC.Ultimate_Net != null)
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
        if(!isLocalPlayer)  return;
        DLog("Start Countdown");
        //Start timer of Timer -- until 0. x.xx 
        tempTimer -= Time.deltaTime;
        if (tempTimer >= 0)
            timeText.text = tempTimer.ToString("f2");
        else
        {
            timeText.text = "0";
            pC.IsDead = true;
            DLog("Kill player");
        }
    }

    /// <summary>
    /// Changes the color of the Player HP Bar on value change
    /// </summary>
    /// <param name="value"></param>
    public void PlayerHPBarColorChange(float value)
    {
        if(!isLocalPlayer)  return;
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
        if(!isLocalPlayer)  return;
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
    /// Changes the color of the Ult meter on value change
    /// </summary>
    /// <param name="value"></param>
    private void UltMeterColorChange(float value)
    {
        if(!isLocalPlayer)  return;
        Color tempColor;

        tempColor = ultFill.color;
        tempColor.g = ((ULT_METER_START_RGB.g * 255) + (-107) * value) / 255;
        tempColor.b = ((ULT_METER_START_RGB.b * 255) + (61) * value) / 255;
        ultFill.color = tempColor;
    }


    [ClientRpc]
    private void RpcUpdateHealth()
    {
        playerHPText.text = PInfo.PlayerHealth.ToString();
        playerMaxHPText.text = "/" + PInfo.PlayerMaxHealth.ToString();
        playerHPSlider.value = PInfo.PlayerHealth / PInfo.PlayerMaxHealth;
    }
    private void DLog(string message)
    {
        if (VERBOSE) LOG_TAG.TPrint(message);
    }
}