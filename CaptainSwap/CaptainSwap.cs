using BepInEx;
//using R2API;
using RoR2;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using System;
using On.EntityStates;
using EntityStates;
using Rewired;


using BepInEx.Configuration;
using EntityStates.Captain.Weapon;
using On.EntityStates.Captain.Weapon;
using On.RoR2.UI;
//using R2API;
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
		public const string PluginVersion = "1.0.0";
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

		}
		
		internal void skilliconhotkeyshow(On.RoR2.UI.SkillIcon.orig_Update orig, RoR2.UI.SkillIcon self)
        {	
			if (self.targetSkill && self.targetSkillSlot == SkillSlot.Utility && self.targetSkill.characterBody.baseNameToken == "CAPTAIN_BODY_NAME")
            {
				self.stockText.SetText("");
				orig.Invoke(self);
				self.stockText.gameObject.SetActive(true);
				self.stockText.fontSize = 12f;		
				self.stockText.SetText( "["+((UnityEngine.KeyCode)captainutilityswapkeycode).ToString() +"]\n"+  (self.stockText.text=="0"?" ": self.stockText.text) , true);
			}
            else
			{ 
				orig.Invoke(self);
			}
		}


		internal void PCMC_stetbodyhook(On.RoR2.PlayerCharacterMasterController.orig_SetBody orig, PlayerCharacterMasterController self, GameObject newbody)
		{
			orig(self, newbody);
			playerbody = newbody;
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
			GenericSkill skill = charbod.skillLocator.GetSkill(SkillSlot.Utility);
			RoR2.Skills.SkillFamily.Variant[] skillvariants = charbod.skillLocator.utility._skillFamily.variants;

			if (skill.skillNameToken == "CAPTAIN_UTILITY_ALT1_NAME")
			{
				diablorecharge = skill.rechargeStopwatch;
				diablostocks = skill.stock;
				charbod.skillLocator.utility.AssignSkill(skillvariants[0].skillDef);
				charbod.skillLocator.GetSkill(SkillSlot.Utility).RemoveAllStocks();
				charbod.skillLocator.GetSkill(SkillSlot.Utility).rechargeStopwatch = proberecharge;
				for (int i = 1; i <= probestocks; i++)
				{
					charbod.skillLocator.GetSkill(SkillSlot.Utility).AddOneStock();
				}

				return 0;
			}
			if (skill.skillNameToken == "CAPTAIN_UTILITY_NAME")
			{
				proberecharge = skill.rechargeStopwatch;
				probestocks = skill.stock;
				charbod.skillLocator.utility.AssignSkill(skillvariants[1].skillDef);
				charbod.skillLocator.GetSkill(SkillSlot.Utility).RemoveAllStocks();
				charbod.skillLocator.GetSkill(SkillSlot.Utility).rechargeStopwatch = diablorecharge;
				for (int i = 1; i <= diablostocks; i++)
				{
					charbod.skillLocator.GetSkill(SkillSlot.Utility).AddOneStock();
				}
				return 1;
			}
			return -1;
		}


	}
}