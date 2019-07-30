using System.ComponentModel;
using UnityEngine;
/// <summary>
/// Collection of Enums to be used throughout the scripts
/// </summary>
namespace Enums
{
    /*
     * User-friendly readable description attributes are added on each value.
     * Use Utility.GetEnumDescription(Enum value) to get the attribute value.
     * e.g. Utility.GetEnumDescription(GameMode.FFA) = Free for All
     */

    public enum Team
    {
        [Description("Choose your team")]
        None,

        [Description("Team 1")]
        Team1, // = 0

        [Description("Team 2")]
        Team2, // = 1

        [Description("Team 3")]
        Team3, // = 2

        [Description("Team 4")]
        Team4  // = 3
    }

    public enum Player
    {
        [Description("Player 1")]
        Player1, // = 0

        [Description("Player 2")]
        Player2, // = 1

        [Description("Player 3")]
        Player3, // = 2

        [Description("Player 4")]
        Player4  // = 3
    }

    public enum GameMode
    {
        [Description("Deathmatch")]
        Deathmatch, // = 0

        [Description("Domination")]
        Domination
    }

    public enum Level
    {

        [Description("StaticLevel")]
        Static, // = 0

        [Description("DynamicLevel")]
        Dyanmic,  // = 1

        [Description("StaticLevelV1")]
        StaticV1, // = 2

        [Description("StaticLevelV2")]
        StaticV2, // = 3

        [Description("TrainingStaticLevel")]
        TrainingStaticLevel
    }

    public enum Weapons
    {
        [Description("Choose your gun")]
        None, 

        [Description("Pump Shotgun")]
        Shotgun,
        
        [Description("Slug Shot")]
        SlugShot,

        [Description("Dragon Breath")]
        FireGun,

        [Description("ShockGun")]
        ShockGun

    }

    public enum Umbrellas
    {
        [Description("Choose your umbrella")]
        None, 

        [Description("Umbrella")]
        Umbrella 

        //[Description("Umbrella 2")]
        //Umbrella2,

        //[Description("Umbrella 3")]
        //Umbrella3
    }

    public enum Ultimates
    {
        [Description("Choose your ultimate")]
        None, 

        [Description("Broken Engine")]
        BrokenEngine,

        [Description("Flamethrower")]
        DragonBreath,

        [Description("Rail Gun")]
        RailGun,
        
        [Description("Tesla Gun")]
        TeslaGun,
        
        [Description("Bubble Gun")]
        BubbleGun
    }

    [DefaultValue(None)]
    public enum Controller
    {
        Controller1,
        Controller2,
        Controller3,
        Controller4,
        None, // ALWAYS keep this one last pls [Kevin]
    }

    public enum Character
    {
        Ace,
        Phil,
        Character3
    }

    public static class IndividualPlayerSetUp
    {
        ///<Summary>
        /// Set up controllers for player
        ///</Summary>
        public static void SetUpPlayer(PlayerController playerController,
                                        PlayerInformation playerInformation,
                                        CameraController cameraController,
                                        Camera UICamera,
                                        Weapon weapon,
                                        Ultimate ultimate,
                                        Player playerNum,
                                        Controller controllerNum)

        {

            // Set team and player number 
            playerInformation.TeamNumber = GameManager.Instance.Players[(int)playerNum].Team;
            playerInformation.PlayerNumber = playerNum;
            //All inputs
            // Analog movements -player and camera
            // Buttons: Down = jump, Right = Umbrella, Up = Ultimate, Left = Reload
            switch (controllerNum)
            {
                case Controller.Controller1:
                    playerController.Horizontal = Axes.Action.MoveXPlayerOne;
                    playerController.Vertical = Axes.Action.MoveYPlayerOne;
                    playerController.JumpButton = Axes.Action.JumpPlayerOne;
                    playerController.ShieldButton = Axes.Action.ShieldPlayerOne;
                    //playerController Aim button for future
                    playerController.UmbrellaButton = Axes.Action.OpenUmbrellaPlayerOne;
                    playerController.StartButton = Axes.Action.StartPlayerOne;

                    cameraController.XAxis = Axes.Action.CamXPlayerOne;
                    cameraController.YAxis = Axes.Action.CamYPlayerOne;


                    weapon.ShootForwardButton = Axes.Action.ShootForwardPlayerOne;
                    weapon.ShootBackwardButton = Axes.Action.ShootBackwardPlayerOne;
                    weapon.ReloadButton = Axes.Action.ReloadPlayerOne;

                    //Add ultimate section
                    ultimate.UltButton = Axes.Action.UltimatePlayerOne;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootForwardButton = Axes.Action.ShootForwardPlayerOne;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootBackwardButton = Axes.Action.ShootBackwardPlayerOne;

                    break;
                case Controller.Controller2:
                    playerController.Horizontal = Axes.Action.MoveXPlayerTwo;
                    playerController.Vertical = Axes.Action.MoveYPlayerTwo;
                    playerController.JumpButton = Axes.Action.JumpPlayerTwo;
                    playerController.ShieldButton = Axes.Action.ShieldPlayerTwo;
                    //playerController Aim button for future
                    playerController.UmbrellaButton = Axes.Action.OpenUmbrellaPlayerTwo;
                    playerController.StartButton = Axes.Action.StartPlayerTwo;
                    
                    cameraController.XAxis = Axes.Action.CamXPlayerTwo;
                    cameraController.YAxis = Axes.Action.CamYPlayerTwo;
                    weapon.ShootForwardButton = Axes.Action.ShootForwardPlayerTwo;
                    weapon.ShootBackwardButton = Axes.Action.ShootBackwardPlayerTwo;
                    weapon.ReloadButton = Axes.Action.ReloadPlayerTwo;

                    //Add ultimate section
                    ultimate.UltButton = Axes.Action.UltimatePlayerTwo;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootForwardButton = Axes.Action.ShootForwardPlayerTwo;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootBackwardButton = Axes.Action.ShootBackwardPlayerTwo;

                    break;
                case Controller.Controller3:
                    playerController.Horizontal = Axes.Action.MoveXPlayerThree;
                    playerController.Vertical = Axes.Action.MoveYPlayerThree;
                    playerController.JumpButton = Axes.Action.JumpPlayerThree;
                    playerController.ShieldButton = Axes.Action.ShieldPlayerThree;
                    //playerController Aim button for future
                    playerController.UmbrellaButton = Axes.Action.OpenUmbrellaPlayerThree;
                    playerController.StartButton = Axes.Action.StartPlayerThree;
                    
                    cameraController.XAxis = Axes.Action.CamXPlayerThree;
                    cameraController.YAxis = Axes.Action.CamYPlayerThree;
                    weapon.ShootForwardButton = Axes.Action.ShootForwardPlayerThree;
                    weapon.ShootBackwardButton = Axes.Action.ShootBackwardPlayerThree;
                    weapon.ReloadButton = Axes.Action.ReloadPlayerThree;

                    //Add ultimate section
                    ultimate.UltButton = Axes.Action.UltimatePlayerThree;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootForwardButton = Axes.Action.ShootForwardPlayerThree;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootBackwardButton = Axes.Action.ShootBackwardPlayerThree;

                    break;
                case Controller.Controller4:
                    playerController.Horizontal = Axes.Action.MoveXPlayerFour;
                    playerController.Vertical = Axes.Action.MoveYPlayerFour;
                    playerController.JumpButton = Axes.Action.JumpPlayerFour;
                    playerController.ShieldButton = Axes.Action.ShieldPlayerFour;
                    //playerController Aim button for future
                    playerController.UmbrellaButton = Axes.Action.OpenUmbrellaPlayerFour;
                    playerController.StartButton = Axes.Action.StartPlayerFour;
                    
                    cameraController.XAxis = Axes.Action.CamXPlayerFour;
                    cameraController.YAxis = Axes.Action.CamYPlayerFour;
                    weapon.ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
                    weapon.ShootBackwardButton = Axes.Action.ShootBackwardPlayerFour;
                    weapon.ReloadButton = Axes.Action.ReloadPlayerFour;

                    //Add ultimate section
                    ultimate.UltButton = Axes.Action.UltimatePlayerFour;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootBackwardButton = Axes.Action.ShootBackwardPlayerFour;

                    break;
                default: // If no controller is set up
                    playerController.Horizontal = Axes.Action.MoveXPlayerFour;
                    playerController.Vertical = Axes.Action.MoveYPlayerFour;
                    playerController.JumpButton = Axes.Action.JumpPlayerFour;
                    playerController.ShieldButton = Axes.Action.ShieldPlayerFour;
                    //playerController Aim button for future
                    playerController.UmbrellaButton = Axes.Action.OpenUmbrellaPlayerFour;
                    playerController.StartButton = Axes.Action.StartPlayerFour;

                    cameraController.XAxis = Axes.Action.CamXPlayerFour;
                    cameraController.YAxis = Axes.Action.CamYPlayerFour;
                    weapon.ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
                    weapon.ShootBackwardButton = Axes.Action.ShootForwardPlayerFour;
                    weapon.ReloadButton = Axes.Action.ReloadPlayerFour;

                    //Add ultimate section
                    ultimate.UltButton = Axes.Action.UltimatePlayerFour;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootForwardButton = Axes.Action.ShootForwardPlayerFour;
                    ultimate.UltWeapon.GetComponent<Weapon>().ShootBackwardButton = Axes.Action.ShootBackwardPlayerFour;
                    break;

            }
        }
        ///<Summary>
        /// Set up Camera position for players
        ///</Summary>
        public static void SetUpCameraPosition(Camera camera, Camera UICamera, Player playerNum, int playerCount, PlayerCanvas playerCanvas)
        {
            float playerCanvasWidth = playerCanvas.transform.GetComponent<RectTransform>().rect.width;
            RectTransform objTeamScore = playerCanvas.transform.Find("Team Score").GetComponent<RectTransform>();
            Vector3 teamScorePos = objTeamScore.anchoredPosition;
            switch (playerCount)
            {
                case 1:
                    Debug.Log("Only 1 player?");
                    break;
                case 2: //Set up player one in left side
                    switch(playerNum)
                    {
                        case Player.Player1:
                            camera.depth = 1;
                            UICamera.depth = 1.1f;
                            camera.rect = new Rect(0,0,0.5f,1); //x, y, width, height
                            UICamera.rect = new Rect(0, 0, 0.5f, 1);
                            break;
                        case Player.Player2:
                            camera.depth = 2;
                            UICamera.depth = 2.1f;
                            camera.rect = new Rect(0.5f,0,0.5f,1); //x, y, width, height
                            UICamera.rect = new Rect(0.5f, 0, 0.5f, 1);

                            objTeamScore.anchoredPosition = new Vector3(-teamScorePos.x, teamScorePos.y, teamScorePos.z);
                            objTeamScore.anchorMin = new Vector2(1, 1);
                            objTeamScore.anchorMax = new Vector2(1, 1);
                            break;
                    }
                    break;
                case 3:
                    switch(playerNum)
                    {
                        case Player.Player1:
                            camera.depth = 1;
                            UICamera.depth = 1.1f;
                            camera.rect = new Rect(0,0.5f,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                            break;
                        case Player.Player2:
                            camera.depth = 2;
                            UICamera.depth = 2.1f;
                            camera.rect = new Rect(0.5f,0.5f,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

                            objTeamScore.anchoredPosition = new Vector3(-teamScorePos.x, teamScorePos.y, teamScorePos.z);
                            objTeamScore.anchorMin = new Vector2(1, 1);
                            objTeamScore.anchorMax = new Vector2(1, 1);
                            break;
                        case Player.Player3:
                            camera.depth = 3;
                            UICamera.depth = 3.1f;
                            camera.rect = new Rect(0,0,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                            break;
                    }
                    break;
                case 4:
                    switch(playerNum)
                    {
                        case Player.Player1:
                            camera.depth = 1;
                            UICamera.depth = 1.1f;
                            camera.rect = new Rect(0,0.5f,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                            break;
                        case Player.Player2:
                            camera.depth = 2;
                            UICamera.depth = 2.1f;
                            camera.rect = new Rect(0.5f,0.5f,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

                            objTeamScore.anchoredPosition = new Vector3(-teamScorePos.x, teamScorePos.y, teamScorePos.z);
                            objTeamScore.anchorMin = new Vector2(1, 1);
                            objTeamScore.anchorMax = new Vector2(1, 1);
                            break;
                        case Player.Player3:
                            camera.depth = 3;
                            UICamera.depth = 3.1f;
                            camera.rect = new Rect(0,0,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                            break;
                        case Player.Player4:
                            camera.depth = 4;
                            UICamera.depth = 4.1f;
                            camera.rect = new Rect(0.5f,0,0.5f,0.5f); //x, y, width, height
                            UICamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);

                            objTeamScore.anchoredPosition = new Vector3(-teamScorePos.x, teamScorePos.y, teamScorePos.z);
                            objTeamScore.anchorMin = new Vector2(1, 1);
                            objTeamScore.anchorMax = new Vector2(1, 1);
                            break;
                    }
                    break;
            }
        }
    }
}
