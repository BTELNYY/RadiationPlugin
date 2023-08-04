using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Helpers;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using PluginAPI.Events;
using InventorySystem.Items.Pickups;
using System.Reflection;

namespace RadiationPlugin
{
    public class Plugin
    {
        public const string PluginName = "RadiationPlugin";
        public const string PluginVersion = "1.0.0";
        public const string PluginDesc = "A plugin to add Radiation into SL, also adds ability to create custom effects..";
        public Harmony harmony;
        public static Plugin instance;
        [PluginConfig(PluginName + ".yml")]
        public Config config = new Config();
        public static Config GetConfig()
        {
            return instance.config;
        }
        public EventHandler eventHandler;
        public string SubclassPath = "";
        public string ConfigPath = "";

        [PluginEntryPoint(PluginName, PluginVersion, PluginDesc, "btelnyy#8395")]
        public void LoadPlugin()
        {
            if (!config.PluginEnabled)
            {
                Log.Debug("Plugin is disabled!");
                return;
            }
            ConfigPath = PluginHandler.Get(this).PluginDirectoryPath;
            instance = this;
            Log.Info("Registering events...");
            PluginAPI.Events.EventManager.RegisterAllEvents(this);
            Log.Info("Patching...");
            harmony = new Harmony("com.btelnyy.radiationplugin");
            harmony.PatchAll();
            Log.Info("RadiationPlugin v" + PluginVersion + " loaded.");
        }

        [PluginUnload()]
        public void Unload()
        {
            config = null;
            eventHandler = null;
            harmony.UnpatchAll("com.btelnyy.radiationplugin"); // this needs to be the same as above or else we unpatch everyone's patches (other plugins) for some godforsaken reason
        }
    }
}
