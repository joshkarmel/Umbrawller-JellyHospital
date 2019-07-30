using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility; //DON'T FORGET THIS
using UnityEngine.UI;
using Enums;

public class PlayerIndicatorPanel : MonoBehaviour
{
    //------------------------------------------------------------------------CONSTANTS:

    public bool VERBOSE = false;

    //---------------------------------------------------------------------------FIELDS:

    [Tooltip("Player Indicator (Target on a player) prefab to load. " +
             "This prefab will be dynamically loaded based on enemies that enter/leave" +
             "the player's view")]
    public GameObject IndicatorPrefab;

    [Tooltip("Player Indicator arrow (Arrow pointing to offscreen player) prefab to load." +
             "This prefab will be dynamically loaded based on enemies that enter/leave" +
             "the player's view")]
    public GameObject ArrowPrefab;

    [Tooltip("Instance of the Player Prefab this panel is parented under")]
    public GameObject PlayerInstance;

    [Tooltip("Instance of the Player Prefab's camera")]
    public Camera PlayerCamera;

    [Tooltip("RectTransform of the PlayerCanvas")]
    public RectTransform CanvasRect;

    [Tooltip("RectTransform of the PlayerArrowPanel so that " +
             "the arrows don't go over the HUD")]
    public RectTransform PlayerArrowPanel;



    private List<GameObject> indicatorPool = new List<GameObject>();
    private int indicatorPoolCursor = 0;

    private List<GameObject> arrowPool = new List<GameObject>();
    private int arrowPoolCursor = 0;

    private PlayerInformation playerInfo;


    //---------------------------------------------------------------------MONO METHODS:

    void Start()
    {
        playerInfo = PlayerInstance.GetComponent<PlayerInformation>();
    }

    void Update()
    {
        DrawIndicators();
    }

    //--------------------------------------------------------------------------METHODS:

    /// <summary>
    /// Resets the cursors pointing to position in the indicatorPool
    /// </summary>
    public void ResetPool()
    {
        indicatorPoolCursor = 0;
        arrowPoolCursor = 0;
    }

    /// <summary>
    /// Returns a player indicator if one exists or instantiates a new one otherwise
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayerIndicator()
    {
        GameObject indicator;
        if(indicatorPoolCursor < indicatorPool.Count)
        {
            indicator = indicatorPool[indicatorPoolCursor]; // Get Existing
        }
        else
        {
            indicator = Instantiate(IndicatorPrefab); // Instantiate a new indicator
            indicator.transform.SetParent(transform); // Put it under this PlayerIndicatorPanel
            indicatorPool.Add(indicator);
        }

        indicatorPoolCursor++;
        return indicator;
    }


    /// <summary>
    /// Returns a player indicator arrow if one exists or instantiates a new one otherwise
    /// </summary>
    public GameObject GetPlayerIndicatorArrow()
    {
        GameObject arrow;
        if (arrowPoolCursor < arrowPool.Count)
        {
            arrow = arrowPool[arrowPoolCursor]; // Get Existing
        }
        else
        {
            arrow = Instantiate(ArrowPrefab); // Instantiate a new indicator
            arrow.transform.SetParent(PlayerArrowPanel.transform); // Put it under the PlayerArrowPanel
            arrowPool.Add(arrow);
        }

        arrowPoolCursor++;
        return arrow;
    }

    /// <summary>
    /// Refreshes the instantiated indicator pools
    /// </summary>
    public void CleanPool()
    {
        while(indicatorPool.Count > indicatorPoolCursor)
        {
            GameObject obj = indicatorPool[indicatorPool.Count - 1]; // Get the last element
            indicatorPool.Remove(obj);
            Destroy(obj.gameObject);
        }

        while (arrowPool.Count > arrowPoolCursor)
        {
            GameObject obj = arrowPool[arrowPool.Count - 1]; // Get the last element
            arrowPool.Remove(obj);
            Destroy(obj.gameObject);
        }
    }

    /// <summary>
    /// Draws indicators for both off-screen players (arrows) and on-screen players (crosshairs)
    /// according to other players' locations relative to the current players camera
    /// </summary>
    public void DrawIndicators()
    {
        ResetPool();

        // Find all the players in the scene
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<PlayerController>() !=  null && obj != PlayerInstance)
            {
                // Translate world coordinates relative to this player's camera
                Vector3 screenPos = PlayerCamera.WorldToViewportPoint(obj.transform.position);
                screenPos.x = ((screenPos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f));
                screenPos.y = ((screenPos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f));

                // Set friendly or enemy color for indicators
                Color colorForIndicator =
                    (playerInfo.TeamNumber == obj.GetComponent<PlayerInformation>().TeamNumber)
                    ? Color.green : Color.red;


                // Other player is on - screen, load an indicator
                if (screenPos.z > 0 && // Is in front of the player
                   screenPos.x > -CanvasRect.rect.width *0.5f && screenPos.x < CanvasRect.rect.width * 0.5f && // Is within horizontal bounds of camera
                   screenPos.y > -CanvasRect.rect.height * 0.5f && screenPos.y < CanvasRect.rect.height * 0.5f) // Is within vertical bounds of camera
                {
                    GameObject indicator = GetPlayerIndicator();
                    indicator.GetComponent<Image>().color = colorForIndicator;
                    indicator.transform.localPosition = screenPos;
                    indicator.transform.localScale = Vector3.one;
                    indicator.transform.localRotation = Quaternion.identity;
                }
                // Off-screen, load an indicator arrow
                else
                {
                    if (screenPos.z < 0)
                        screenPos *= -1;

                    Vector3 screenCenter = new Vector3(PlayerArrowPanel.rect.width, PlayerArrowPanel.rect.height, 0) / 2;

                    float angle = Mathf.Atan2(screenPos.y, screenPos.x);
                    angle -= 90 * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = -Mathf.Sin(angle);
                    float m = cos / sin;

                    Vector3 screenBounds = screenCenter * 0.9f;

                    if (cos > 0)
                    {
                        screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
                    }
                    else
                    {
                        //down
                        screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
                    }

                    // if out of bounds, get point on appropriate side
                    if (screenPos.x > screenBounds.x) // out of bounds! must be on the right
                    {
                        screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
                    }
                    else if (screenPos.x < -screenBounds.x)
                    {
                        screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
                    } // else in bounds

                    GameObject arrow = GetPlayerIndicatorArrow();
                    arrow.GetComponent<Image>().color = colorForIndicator;
                    arrow.transform.localScale = Vector3.one;
                    arrow.transform.localPosition = screenPos;
                    arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                }

            }
        }
        CleanPool();
    }

    

    //--------------------------------------------------------------------------HELPERS:

    private void DLog(string message)
    {
        if (VERBOSE) this.GetType().Name.TPrint(message);
    }
}
