using BingoGoalPack1.CustomVariables;
using BingoSyncExtension;
using Modding;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonoMod.Utils;
using MonoMod.RuntimeDetour;
using System.IO;
using System;

namespace BingoGoalPack1 {
    public class BingoGoalPack1: Mod{
        new public string GetName() => "BingoGoalPack1";
        public override string GetVersion() => "1.3.1.0";
        public override int LoadPriority() => 8;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            VariableProxy.Setup(Log);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //set up goal lists
            Dictionary<string, BingoGoal> vanillaGoals = GameModesManager.GetVanillaGoals();

            BingoSquareInjector.ProcessGoalsFile(path + "/Squares/Extended.json");
            Dictionary<string, BingoGoal> extendedGoals = setupExtendedDict();
            GameModesManager.RegisterGoalsForCustom("Extended", extendedGoals);
            Dictionary<string, BingoGoal> extendedPlusGoals = setupExtendedPlusDict();
            GameModesManager.RegisterGoalsForCustom("Extended+", extendedPlusGoals);

            extendedGoals.AddRange(vanillaGoals);
            GameMode mode_extended = new GameMode("Extended", extendedGoals);
            GameModesManager.AddGameMode(mode_extended);

            extendedPlusGoals.AddRange(extendedGoals);
            GameMode mode_extendedPlus = new GameMode("Extended+", extendedPlusGoals);
            GameModesManager.AddGameMode(mode_extendedPlus);

            Dictionary<string, BingoGoal> hardsaveGoals = BingoSquareInjector.ProcessGoalsFile(path + "/Squares/BenchBingo.json");
            setupHardsaveDict(hardsaveGoals);
            GameMode mode_hardsaves = new GameMode("Hardsaves", hardsaveGoals);
            GameModesManager.AddGameMode(mode_hardsaves);

            Dictionary<string, BingoGoal> grubGoals = BingoSquareInjector.ProcessGoalsFile(path + "/Squares/GrubBingo.json");
            GameMode mode_grubs = new GameMode("Grubs", grubGoals);
            GameModesManager.AddGameMode(mode_grubs);

            BingoSquareInjector.ProcessGoalsFile(path + "/Squares/GodhomeBingo.json");
            GameMode mode_godhome = new GodhomeMode();
            GameModesManager.AddGameMode(mode_godhome);

            Dictionary<string, BingoGoal> relicGoals = BingoSquareInjector.ProcessGoalsFile(path + "/Squares/RelicBingo.json");
            setupRelicDict(relicGoals);
            GameMode mode_relics = new GameMode("Relics", relicGoals);
            GameModesManager.AddGameMode(mode_relics);

            //add hooks
            On.GameManager.AwardAchievement += Neglect.CheckNeglectAchievement;
            ModHooks.SetPlayerBoolHook += CoreShard.CheckIfCoreShardWasCollected;
            On.PlayMakerFSM.OnEnable += Totems.CreateSoulTotemTrigger;
            On.DialogueBox.StartConversation += DialogueExtension.StartConversation;
            ModHooks.SetPlayerIntHook += GrubsExtension.CheckIfGrubWasSaved;
            On.PlayMakerFSM.OnEnable += Diary.CreateDiaryTrigger;
            On.PlayMakerFSM.OnEnable += Arenas.CreateArenaTrigger;
            On.PlayMakerFSM.OnEnable += ChestsExtension.CreateJunkpitChestTrigger;
            On.PlayMakerFSM.OnEnable += Hardsaves.CreateHardsaveTrigger;
            On.RespawnTrigger.OnTriggerEnter2D += Hardsaves.CreateRespawnTriggerTrigger;
            On.PlayMakerFSM.OnEnable += GrubsExtension.SaveOneGrub;
            On.PlayMakerFSM.OnEnable += Ghosts.CreateGhostKilledTrigger;
            On.PlayMakerFSM.OnEnable += DreamNailMace.CreateMaceTrigger;
            On.BossStatue.SetPlaqueState += HallOfGodsTracker.CreateHogStatueTrigger;
            On.PlayMakerFSM.OnEnable += Sit.CreateSitTrigger;

            //dream tree logic
            var _hook = new ILHook
            (
                typeof(DreamPlant).GetMethod("CheckOrbs", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(),
                DreamTreesExtension.TrackDreamTrees
            );

            Log("Initialized");
        }

        private Dictionary<string, BingoGoal> setupExtendedDict() {
            Dictionary<string, BingoGoal> output = new Dictionary<string, BingoGoal>();
            string[] goals = { "Buy the QG and Fog Canyon maps", "Collect 4 Simple Keys", "Complete the Ancestral Mound Root", "Complete the Greenpath Root", "Decipher Hunter's Notes: Maskfly + Shrumeling","Dream Nail Poggy Thorax and Blue Child Joni", "Fungal Core Mask Shard", "Give Divine a Charm", "Hallownest Crown Pale Ore", "Interact with Cornifer in Waterways", "Obtain Godtuner", "Open 3 geo chests (not in junk pit)", "Open 6 geo chests (not in junk pit)", "Read Bretta's diary", "Save the 2 grubs in Basin", "Save the 2 grubs in Hive", "Slash Millibelle in Pleasure House", "Slash Zote's corpse in Greenpath", "Soul Eater + Soul Catcher", "Talk to Tuk" };
            foreach(string goal in goals) {
                output.Add(goal, new BingoGoal(goal));
            }
            output["Collect 4 Simple Keys"].exclusions.Add("Pale Lurker" );
            output["Dream Nail Poggy Thorax and Blue Child Joni"].exclusions.Add("10 Lifeblood masks at the same time");
            output["Dream Nail Poggy Thorax and Blue Child Joni"].exclusions.Add("Lifeblood Heart + Joni's Blessing" );
            output["Fungal Core Mask Shard"].exclusions.Add("Bow to the Fungal Core Elder" );
            output["Hallownest Crown Pale Ore"].exclusions.Add("Have 2 Pale Ore");
            output["Hallownest Crown Pale Ore"].exclusions.Add("Nail 2" );
            output["Obtain Godtuner"].exclusions.Add("Nothing? (junk pit chest)" );
            output["Read Bretta's diary"].exclusions.Add("Rescue Bretta + Sly" );
            output["Save the 2 grubs in Hive"].exclusions.Add("Mask Shard  in the Hive" );
            output["Slash Millibelle in Pleasure House"].exclusions.Add("Dream Nail Marissa" );
            output["Talk to Tuk"].exclusions.Add("Have 4 Rancid Eggs" );
            return output;
        }

        private Dictionary<string, BingoGoal> setupExtendedPlusDict() {
            Dictionary<string, BingoGoal> output = new Dictionary<string, BingoGoal>();
            string[] goals = { "10 Lifeblood masks at the same time", "All Fungal Benches", "All Greenpath Benches", "Both Waterways Arenas", "Bow to Moss Prophet, dead or alive", "Bow to the Fungal Core Elder", "Buy 4 map markers from Iselda (All)", "Charged Lumafly Journal Entry", "Check the journal below Stone Sanctuary", "Defeat 3 dream warriors", "Destroy every tablet in the Shrine of Believers", "Dream Nail both Mace Bug + Leg Eater Bench", "Dream Nail Marissa", "Dream Nail Willoh's meal", "Equip 5 Charms at the same time", "Hit the Oro scarecrow up until the hoppers spawn", "Interact with 4 Quirrel locations (outside Archives)", "Kill 10 Mantis Petras", "Kill 3 Flukemungas", "Kill a Durandoo", "Kill a Lightseed", "Kill the Mossy Vagabonds", "Kill three different Great Husk Sentries", "Kill two different Alubas", "Look through Lurien's telescope", "Nailmaster's Glory", "Nothing? (junk pit chest)", "Obtain World Sense", "Open the Dirtmouth / Crystal Peak elevator", "Sit at 4 Toll Benches", "Sit down in Failed Tramway", "Sit with Mato, Marissa, and Quirrel", "Slash 10 different soul totems", "Slash two Shade Gates", "Swat Tiso's shield away from his corpse", "Talk to Divine with Crest Equipped", "Visit all 4 shops (Sly, Iselda, Salubra and Leg Eater)" };
            foreach(string goal in goals) {
                output.Add(goal, new BingoGoal(goal));
            }
            output["10 Lifeblood masks at the same time"].exclusions.Add("Lifeblood Heart + Joni's Blessing");
            output["10 Lifeblood masks at the same time"].exclusions.Add("Dream Nail Poggy Thorax and Blue Child Joni" );
            output["Both Waterways Arenas"].exclusions.Add("Interact with Cornifer in Waterways");
            output["Bow to Moss Prophet, dead or alive"].exclusions.Add( "Kill the Mossy Vagabonds" );
            output["Bow to the Fungal Core Elder"].exclusions.Add( "Fungal Core Mask Shard" );
            output["Buy 4 map markers from Iselda (All)"].exclusions.Add("Buy 6 map pins from Iselda (All but two)");
            output["Buy 4 map markers from Iselda (All)"].exclusions.Add("Buy 8 map pins from Iselda (All)");
            output["Defeat 3 dream warriors"].exclusions.Add("Collect 500 essence");
            output["Defeat 3 dream warriors"].exclusions.Add("Dream Wielder" );
            output["Dream Nail Marissa"].exclusions.Add("Slash Millibelle in Pleasure House" );
            output["Equip 5 Charms at the same time"].exclusions.Add("Obtain 3 extra notches" );
            output["Hit the Oro scarecrow up until the hoppers spawn"].exclusions.Add("Dash Slash" );
            output["Kill the Mossy Vagabonds"].exclusions.Add("Bow to Moss Prophet, dead or alive" );
            output["Look through Lurien's telescope"].exclusions.Add("Lurien" );
            output["Nothing? (junk pit chest)"].exclusions.Add("Obtain Godtuner" );
            output["Open the Dirtmouth / Crystal Peak elevator"].exclusions.Add("Kill 4 Mimics" );
            return output;
        }

        private void setupHardsaveDict(Dictionary<string, BingoGoal> goals) {
            goals["Beast's Den / Herrah hardsave"].exclusions.Add("Beast's Den bench");
            goals["Beast's Den bench"].exclusions.Add("Beast's Den / Herrah hardsave");
        }

        private void setupRelicDict(Dictionary<string, BingoGoal> goals) {
            goals["Journal: Crypts"].exclusions.Add("Seal: Crypts");
            goals["Journal: Kingdom's Edge above Cornifer"].exclusions.Add("Seal: King's Station");
            goals["Journal: Below Shrumal Ogres"].exclusions.Add("Seal: Sporgs");
            goals["Journal: Howling Cliffs"].exclusions.Add("Idol: Howling Cliffs");
            goals["Journal: Crystal Peak conga line"].exclusions.Add("Idol: Crystal Peak Cornifer");
            goals["Seal: Sporgs"].exclusions.Add("Journal: Below Shrumal Ogres");
            goals["Seal: Crypts"].exclusions.Add("Journal: Crypts");
            goals["Seal: King's Station"].exclusions.Add("Journal: Kingdom's Edge above Cornifer");
            goals["Seal: 100 essence"].exclusions.Add("Idol: Spirits' Glade");
            goals["Idol: Crystal Peak Cornifer"].exclusions.Add("Journal: Crystal Peak conga line");
            goals["Idol: Spirits' Glade"].exclusions.Add("Seal: 100 essence");
            goals["Idol: Howling Cliffs"].exclusions.Add("Journal: Howling Cliffs");
            goals["Idol: Below Colosseum"].exclusions.Add("Idol: Pale Lurker");
            goals["Idol: Pale Lurker"].exclusions.Add("Idol: Below Colosseum");
        }
    }
}