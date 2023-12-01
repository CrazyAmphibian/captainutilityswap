using BepInEx;
//using R2API;
using RoR2;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using System;
using On.EntityStates;
using EntityStates;
using BepInEx.Configuration;
using On.RoR2.UI;
using RoR2.UI;


namespace CaptainSwap
{
	//const int diabloskillid=148

	[BepInPlugin(PluginGUID, PluginName, PluginVersion)]






	public class Plugin : BaseUnityPlugin
	{
		public const string PluginGUID = PluginAuthor + "." + PluginName;
		public const string PluginAuthor = "CrazyAmphibian";
		public const string PluginName = "CaptainSwap";
		public const string PluginVersion = "1.2.0";
		//PlayerCharacterMasterController playerCharacterMasterController;
		GameObject playerbody;


		public abstract class BaseUnityPlugin : MonoBehaviour
		{
			protected ConfigFile Config { get; }
			public static ConfigEntry<int> captainutilityswapkeycode { get; set; }


		}

		


		public void Awake()
		{
			Log.Init(Logger);
			captainutilityswapkeycode = Config.Bind<int>(
				"input",
				"hotkey (unity keycode)",
				(int)KeyCode.T,
				"which keypress will swap utilities.\nTakes the form of a unity keycode.").Value;

			On.RoR2.PlayerCharacterMasterController.SetBody += PCMC_stetbodyhook;	
			On.RoR2.UI.SkillIcon.Update += skilliconhotkeyshow;
			On.RoR2.Run.BeginStage += refreshallcaptainutilities;
		}
		
		internal void skilliconhotkeyshow(On.RoR2.UI.SkillIcon.orig_Update orig, RoR2.UI.SkillIcon self)
        {
			orig(self);
			if (self.targetSkill && self.targetSkillSlot == SkillSlot.Utility && self.targetSkill.characterBody.baseNameToken == "CAPTAIN_BODY_NAME")
            {
				self.stockText.gameObject.SetActive(true);
				self.stockText.fontSize = 12f;		
				self.stockText.SetText( "["+((UnityEngine.KeyCode)captainutilityswapkeycode).ToString() +"]\n"+ self.targetSkill.stock.ToString(), true);
			}
            else
			{ 
			}
		}


		internal void PCMC_stetbodyhook(On.RoR2.PlayerCharacterMasterController.orig_SetBody orig, PlayerCharacterMasterController self, GameObject newbody)
		{
			orig(self, newbody);
			if (newbody)
			{
				CharacterBody chbd = newbody.GetComponent<CharacterBody>();
				if (chbd && chbd.baseNameToken == "CAPTAIN_BODY_NAME" && chbd.localPlayerAuthority)
				{
					//Log.Info("### A character body was created. " + chbd.name + " " + chbd.isLocalPlayer.ToString() + " " + chbd.playerControllerId.ToString() + " " + chbd.networkIdentity.isClient.ToString() + " " + chbd.localPlayerAuthority.ToString() + " " + chbd.netId.ToString());
					playerbody = newbody;
				}
			}
		}

		int probestocks = 1;
		int diablostocks = 1;
		float proberecharge = 0f;
		float diablorecharge = 0f;
		public int captainutilityswapkeycode = (int)KeyCode.T;
		bool pressedlastframe = false;
		bool pressedthisframe = false;
		public void FixedUpdate()
		{
			if (playerbody)
			{

				CharacterBody charbod = playerbody.GetComponent<CharacterBody>();
				if (charbod && charbod.baseNameToken == "CAPTAIN_BODY_NAME")
				{
					pressedthisframe= UnityEngine.Input.GetKey((UnityEngine.KeyCode)captainutilityswapkeycode);

					if (pressedthisframe && (!pressedlastframe))
					{
						swapcaptainutilityskills(charbod);
					}

					pressedlastframe = pressedthisframe;


				}
			}
		}



		public int swapcaptainutilityskills(CharacterBody charbod)
        {
			GenericSkill skill = charbod.skillLocator.utility;
			RoR2.Skills.SkillFamily.Variant[] skillvariants = charbod.skillLocator.utility?._skillFamily.variants;

			if (skill?.skillNameToken == "CAPTAIN_UTILITY_ALT1_NAME") //if we're using the diablo strike
			{
				diablorecharge = skill.rechargeStopwatch;
				diablostocks = skill.stock;
				charbod.skillLocator.utility.AssignSkill(skillvariants[0].skillDef);
				charbod.skillLocator.utility.RemoveAllStocks();
				for (int i = 1; i <= probestocks; i++)
				{
					charbod.skillLocator.utility.AddOneStock();
				}
				charbod.skillLocator.utility.rechargeStopwatch = proberecharge;

				return 0;
			}
			if (skill?.skillNameToken == "CAPTAIN_UTILITY_NAME") //if we're using the orbital probe
			{
				proberecharge = skill.rechargeStopwatch;
				probestocks = skill.stock;
				charbod.skillLocator.utility.AssignSkill(skillvariants[1].skillDef);
				charbod.skillLocator.utility.RemoveAllStocks();
				for (int i = 1; i <= diablostocks; i++)
				{
					charbod.skillLocator.utility.AddOneStock();
				}
				charbod.skillLocator.utility.rechargeStopwatch = diablorecharge;
				return 1;
			}
			return -1;
		}


		public void refreshallcaptainutilities(On.RoR2.Run.orig_BeginStage orig, RoR2.Run self)
        {
			orig(self);
			if (playerbody){
				CharacterBody charbod = playerbody.GetComponent<CharacterBody>();
				if (charbod && charbod.baseNameToken == "CAPTAIN_BODY_NAME")
				{
					Log.Debug("Refilling for end of scene...");
					charbod.skillLocator.utility?.Reset(); //run twice so we refresh each.
					swapcaptainutilityskills(charbod);
					charbod.skillLocator.utility?.Reset();
					swapcaptainutilityskills(charbod);
					
				}
			}

		}

	}
}