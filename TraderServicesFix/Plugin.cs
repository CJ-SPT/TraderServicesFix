using BepInEx;
using DrakiaXYZ.VersionChecker;
using EFT;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UseItemsFromAnywhere
{
    [BepInPlugin("com.dirtbikercj.TraderServicesFix", "TraderServicesFix", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public const int TarkovVersion = 30626;

        public static Plugin instance;

        private static Dictionary<string, Profile.ETraderServiceSource> _traderIdToTraderService = new()
        {
            {
                "5ac3b934156ae10c4430e83c",
                Profile.ETraderServiceSource.Ragman
            },
            {
                "6617beeaa9cfa777ca915b7c",
                Profile.ETraderServiceSource.ArenaManager
            }
        };

        private void Awake()
        {
            if (!VersionChecker.CheckEftVersion(Logger, Info, Config))
            {
                throw new Exception("Invalid EFT Version");
            }

            instance = this;
            DontDestroyOnLoad(this);

            LoadTraderServices();

            var traderIdToTraderService = AccessTools.Field(typeof(Profile.TraderInfo), "TraderIdToTraderService");

            traderIdToTraderService.SetValue(traderIdToTraderService, _traderIdToTraderService);
        }

        private static void LoadTraderServices()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var jsonDir = Path.Combine(assemblyPath, "Traders");

            foreach (var file in Directory.GetFiles(jsonDir, "*.json"))
            {
                var json = File.ReadAllText(file);

                var id = Path.GetFileNameWithoutExtension(file);
                var service = JsonConvert.DeserializeObject<TraderServiceModel>(json);

                if (id != null && service != null)
                {
                    _traderIdToTraderService.Add(id, Profile.ETraderServiceSource.Ragman);
                }
            }
        }
    }

    public class TraderServiceModel
    {
        public bool ClothingService { get; set; }
    }
}