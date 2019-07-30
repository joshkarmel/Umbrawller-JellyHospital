using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
public class Axes : Singleton<Axes> 
{
//------------------------------------------------------------------------CONSTANTS:
	public enum Action 
	{
		MoveXPlayerOne, MoveYPlayerOne,
		CamXPlayerOne, CamYPlayerOne,
		ShootForwardPlayerOne,  ShootBackwardPlayerOne, ReloadPlayerOne,
		JumpPlayerOne, ShieldPlayerOne, AimPlayerOne, OpenUmbrellaPlayerOne,

		MoveXPlayerTwo, MoveYPlayerTwo,
		CamXPlayerTwo, CamYPlayerTwo,
		ShootForwardPlayerTwo, ShootBackwardPlayerTwo, ReloadPlayerTwo,
		JumpPlayerTwo, ShieldPlayerTwo, AimPlayerTwo, OpenUmbrellaPlayerTwo,

		MoveXPlayerThree, MoveYPlayerThree,
		CamXPlayerThree, CamYPlayerThree,
		ShootForwardPlayerThree, ShootBackwardPlayerThree, ReloadPlayerThree,
		JumpPlayerThree, ShieldPlayerThree, AimPlayerThree, OpenUmbrellaPlayerThree,

		MoveXPlayerFour, MoveYPlayerFour,
		CamXPlayerFour, CamYPlayerFour,
		ShootForwardPlayerFour, ShootBackwardPlayerFour, ReloadPlayerFour,
		JumpPlayerFour, ShieldPlayerFour, AimPlayerFour, OpenUmbrellaPlayerFour,

		Horizontal, Vertical,
		MouseX, MouseY,
		Fire1,  Fire2, //Shoot1 = forward = left click, shoot2 = backwards = right click
		Jump, Shield, Aim, OpenUmbrella,

		//Because everything will break if I add them above, I will add them below
		StartPlayerOne, StartPlayerTwo, StartPlayerThree, StartPlayerFour, Escape,
		
		UltimatePlayerOne, UltimatePlayerTwo, UltimatePlayerThree, UltimatePlayerFour, Ultimate,
		LBPlayer1, LBPlayer2, LBPlayer3, LBPlayer4
	}
//---------------------------------------------------------------------------FIELDS:
	
//---------------------------------------------------------------------MONO METHODS:

//--------------------------------------------------------------------------METHODS:
	public static Dictionary<Action, string> toStr = new Dictionary<Action, string>
	{
		{Action.MoveXPlayerOne,"Horizontal PlayerOne"},
		{Action.MoveYPlayerOne,"Vertical PlayerOne"},
		{Action.CamXPlayerOne,"Cam Horizontal PlayerOne"},
		{Action.CamYPlayerOne,"Cam Vertical PlayerOne"},
		{Action.ShootForwardPlayerOne, "ShootForwardPlayerOne"},
		{Action.ShootBackwardPlayerOne, "ShootBackwardPlayerOne"},
		{Action.ReloadPlayerOne, "ReloadPlayerOne"},
		{Action.JumpPlayerOne, "JumpPlayerOne"},
		{Action.ShieldPlayerOne, "ShieldPlayerOne"},
		{Action.AimPlayerOne, "AimPlayerOne"},
		{Action.OpenUmbrellaPlayerOne, "OpenUmbrellaPlayerOne"},
        {Action.LBPlayer1, "LB PlayerOne" },

		{Action.MoveXPlayerTwo,"Horizontal PlayerTwo"},
		{Action.MoveYPlayerTwo,"Vertical PlayerTwo"},
		{Action.CamXPlayerTwo,"Cam Horizontal PlayerTwo"},
		{Action.CamYPlayerTwo,"Cam Vertical PlayerTwo"},
		{Action.ShootForwardPlayerTwo, "ShootForwardPlayerTwo"},
		{Action.ShootBackwardPlayerTwo, "ShootBackwardPlayerTwo"},
		{Action.ReloadPlayerTwo, "ReloadPlayerTwo"},
		{Action.JumpPlayerTwo, "JumpPlayerTwo"},
		{Action.ShieldPlayerTwo, "ShieldPlayerTwo"},
		{Action.AimPlayerTwo, "AimPlayerTwo"},
		{Action.OpenUmbrellaPlayerTwo, "OpenUmbrellaPlayerTwo"},
        {Action.LBPlayer2, "LB PlayerTwo"},

		{Action.MoveXPlayerThree,"Horizontal PlayerThree"},
		{Action.MoveYPlayerThree,"Vertical PlayerThree"},
		{Action.CamXPlayerThree,"Cam Horizontal PlayerThree"},
		{Action.CamYPlayerThree,"Cam Vertical PlayerThree"},
		{Action.ShootForwardPlayerThree, "ShootForwardPlayerThree"},
		{Action.ShootBackwardPlayerThree, "ShootBackwardPlayerThree"},
		{Action.ReloadPlayerThree, "ReloadPlayerThree"},
		{Action.JumpPlayerThree, "JumpPlayerThree"},
		{Action.ShieldPlayerThree, "ShieldPlayerThree"},
		{Action.AimPlayerThree, "AimPlayerThree"},
		{Action.OpenUmbrellaPlayerThree, "OpenUmbrellaPlayerThree"},
        {Action.LBPlayer3, "LB PlayerThree"},

		{Action.MoveXPlayerFour,"Horizontal PlayerFour"},
		{Action.MoveYPlayerFour,"Vertical PlayerFour"},
		{Action.CamXPlayerFour,"Cam Horizontal PlayerFour"},
		{Action.CamYPlayerFour,"Cam Vertical PlayerFour"},
		{Action.ShootForwardPlayerFour, "ShootForwardPlayerFour"},
		{Action.ShootBackwardPlayerFour, "ShootBackwardPlayerFour"},
		{Action.ReloadPlayerFour, "ReloadPlayerFour"},
		{Action.JumpPlayerFour, "JumpPlayerFour"},
		{Action.ShieldPlayerFour, "ShieldPlayerFour"},
		{Action.AimPlayerFour, "AimPlayerFour"},
		{Action.OpenUmbrellaPlayerFour, "OpenUmbrellaPlayerFour"},
        {Action.LBPlayer4, "LB PlayerFour"},

		{Action.Horizontal,"Horizontal"},
		{Action.Vertical,"Vertical"},
		{Action.MouseX,"Mouse X"},
		{Action.MouseY,"Mouse Y"},
		{Action.Fire1, "Fire1"},
		{Action.Fire2, "Fire2"},
		{Action.Jump, "Jump"},
		{Action.Shield, "Shield"},
		{Action.Aim, "Aim"},
		{Action.OpenUmbrella, "OpenUmbrella"},

		//Start button
		{Action.StartPlayerOne, "Start PlayerOne"},
		{Action.StartPlayerTwo, "Start PlayerTwo"},
		{Action.StartPlayerThree, "Start PlayerThree"},
		{Action.StartPlayerFour, "Start PlayerFour"},
		{Action.Escape, "Escape"},

		//Ultimate button
		{Action.UltimatePlayerOne, "Ultimate PlayerOne"},
		{Action.UltimatePlayerTwo, "Ultimate PlayerTwo"},
		{Action.UltimatePlayerThree, "Ultimate PlayerThree"},
		{Action.UltimatePlayerFour, "Ultimate PlayerFour"},
		{Action.Ultimate, "Ultimate"},
	};

	public static float GetAxis(Action a) 
	{
		return Input.GetAxis(toStr[a]);
	}
//--------------------------------------------------------------------------HELPERS:
	
}