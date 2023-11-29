using BepInEx;
//using R2API;
using RoR2;
using UnityEngine;
//using UnityEngine.AddressableAssets;

namespace StageProgression
{

	[BepInPlugin(PluginGUID, PluginName, PluginVersion)]

	public class Plugin : BaseUnityPlugin
	{
		public const string PluginGUID = PluginAuthor + "." + PluginName;
		public const string PluginAuthor = "CrazyAmphibian";
		public const string PluginName = "CaptainSwap";
		public const string PluginVersion = "1.0.0";

		public void Awake()
		{
			Log.Init(Logger);
			

			
			
		}
	}


}