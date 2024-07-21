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
using System.Linq;

namespace BingoGoalPack1 {
    public class BingoGoalPack1: Mod{
        new public string GetName() => "BingoGoalPack1";
        public override string GetVersion() => "1.4.0.0";
        public override int LoadPriority() => 8;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            VariableProxy.Setup(Log);
            Assembly assembly = Assembly.GetExecutingAssembly();

            //set up goal lists
            Dictionary<string, BingoGoal> vanillaGoals = GameModesManager.GetVanillaGoals();

            processEmbeddedJson(assembly, "Extended");
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

            Dictionary<string, BingoGoal> hardsaveGoals = processEmbeddedJson(assembly, "BenchBingo");
            setupHardsaveDict(hardsaveGoals);
            GameMode mode_hardsaves = new GameMode("Hardsaves", hardsaveGoals);
            GameModesManager.AddGameMode(mode_hardsaves);

            Dictionary<string, BingoGoal> grubGoals = processEmbeddedJson(assembly, "GrubBingo");
            setupGrubDict(grubGoals);
            GameMode mode_grubs = new GameMode("Grubs", grubGoals);
            GameModesManager.AddGameMode(mode_grubs);

            processEmbeddedJson(assembly, "GodhomeBingo");
            GameMode mode_godhome = new GodhomeMode();
            GameModesManager.AddGameMode(mode_godhome);

            Dictionary<string, BingoGoal> relicGoals = processEmbeddedJson(assembly, "RelicBingo");
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

        private Dictionary<string, BingoGoal> processEmbeddedJson(Assembly assembly, string jsonName) {
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Squares." + jsonName + ".json"));
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return BingoSquareInjector.ProcessGoalsFile(stream);
        }

        private Dictionary<string, BingoGoal> setupExtendedDict() {
            Dictionary<string, BingoGoal> output = new Dictionary<string, BingoGoal>();
            foreach(string goal in goalEnum.extended.extendedList) {
                output.Add(goal, new BingoGoal(goal));
            }
            output[goalEnum.extended.fourKeys].exclusions.Add(goalEnum.vanilla.paleLurker);
            output[goalEnum.extended.poggyJoni].exclusions.Add(goalEnum.extended.lifeblood10);
            output[goalEnum.extended.fungalShard].exclusions.Add(goalEnum.extended.fungalElder);
            output[goalEnum.extended.crownOre].exclusions.Add(goalEnum.vanilla.paleOre);
            output[goalEnum.extended.crownOre].exclusions.Add(goalEnum.vanilla.nail2);
            output[goalEnum.extended.godtuner].exclusions.Add(goalEnum.extended.nothing);
            output[goalEnum.extended.diary].exclusions.Add(goalEnum.vanilla.brettaSly);
            output[goalEnum.extended.grubsHive].exclusions.Add(goalEnum.vanilla.hiveShard);
            output[goalEnum.extended.slashMillibelle].exclusions.Add(goalEnum.extended.marissa);
            output[goalEnum.extended.tuk].exclusions.Add(goalEnum.vanilla.fourEggs);
            return output;
        }

        private Dictionary<string, BingoGoal> setupExtendedPlusDict() {
            Dictionary<string, BingoGoal> output = new Dictionary<string, BingoGoal>();
            foreach(string goal in goalEnum.extended.extendedPlusList) {
                output.Add(goal, new BingoGoal(goal));
            }
            output[goalEnum.extended.lifeblood10].exclusions.Add(goalEnum.vanilla.lifeblood);
            output[goalEnum.extended.lifeblood10].exclusions.Add(goalEnum.extended.poggyJoni);
            output[goalEnum.extended.waterwaysArenas].exclusions.Add(goalEnum.extended.waterwaysCornifer);
            output[goalEnum.extended.mossProphet].exclusions.Add(goalEnum.extended.vagabonds);
            output[goalEnum.extended.fungalElder].exclusions.Add(goalEnum.extended.fungalShard);
            output[goalEnum.extended.markers4].exclusions.Add(goalEnum.vanilla.pins6);
            output[goalEnum.extended.markers4].exclusions.Add(goalEnum.vanilla.pins8);
            output[goalEnum.extended.dreamWarriors3].exclusions.Add(goalEnum.vanilla.essence);
            output[goalEnum.extended.dreamWarriors3].exclusions.Add(goalEnum.vanilla.wielder);
            output[goalEnum.extended.shrineTablets].exclusions.Add(goalEnum.vanilla.revek);
            output[goalEnum.extended.marissa].exclusions.Add(goalEnum.extended.slashMillibelle);
            output[goalEnum.extended.equip5Charms].exclusions.Add(goalEnum.vanilla.notches);
            output[goalEnum.extended.scarecrow].exclusions.Add(goalEnum.vanilla.dashSlash);
            output[goalEnum.extended.vagabonds].exclusions.Add(goalEnum.extended.mossProphet);
            output[goalEnum.extended.telescope].exclusions.Add(goalEnum.vanilla.lurien);
            output[goalEnum.extended.nothing].exclusions.Add(goalEnum.extended.godtuner);
            output[goalEnum.extended.dirtmouthElevator].exclusions.Add(goalEnum.vanilla.mimics);
            output[goalEnum.extended.tollBenches].exclusions.Add(goalEnum.vanilla.sixTolls);
            return output;
        }

        private void setupHardsaveDict(Dictionary<string, BingoGoal> goals) {
            goals[goalEnum.hardsaves.archives].exclusions.Add(goalEnum.grubs.archives);
            goals[goalEnum.hardsaves.basinToll].exclusions.Add(goalEnum.extended.grubsBasin);
            goals[goalEnum.hardsaves.basinToll].exclusions.Add(goalEnum.extended.tollBenches);
            goals[goalEnum.hardsaves.basinToll].exclusions.Add(goalEnum.grubs.basinDive);
            goals[goalEnum.hardsaves.basinToll].exclusions.Add(goalEnum.grubs.basinWings);
            goals[goalEnum.hardsaves.denHerrah].exclusions.Add(goalEnum.hardsaves.denBench);
            goals[goalEnum.hardsaves.denHerrah].exclusions.Add(goalEnum.vanilla.herrah);
            goals[goalEnum.hardsaves.denHerrah].exclusions.Add(goalEnum.vanilla.hornetHerrah);
            goals[goalEnum.hardsaves.denHerrah].exclusions.Add(goalEnum.relics.sealDen);
            goals[goalEnum.hardsaves.denBench].exclusions.Add(goalEnum.hardsaves.denHerrah);
            goals[goalEnum.hardsaves.denBench].exclusions.Add(goalEnum.vanilla.herrah);
            goals[goalEnum.hardsaves.denBench].exclusions.Add(goalEnum.vanilla.hornetHerrah);
            goals[goalEnum.hardsaves.bretta].exclusions.Add(goalEnum.extended.fungalBenches);
            goals[goalEnum.hardsaves.camp].exclusions.Add(goalEnum.vanilla.hornet2);
            goals[goalEnum.hardsaves.camp].exclusions.Add(goalEnum.relics.journalCamp);
            goals[goalEnum.hardsaves.camp].exclusions.Add(goalEnum.hardsaves.brand);
            goals[goalEnum.hardsaves.cityGate].exclusions.Add(goalEnum.relics.sealRafters);
            goals[goalEnum.hardsaves.cityGate].exclusions.Add(goalEnum.hardsaves.cityQuirrel);
            goals[goalEnum.hardsaves.cityQuirrel].exclusions.Add(goalEnum.hardsaves.cityGate);
            goals[goalEnum.hardsaves.cityQuirrel].exclusions.Add(goalEnum.relics.sealRafters);
            goals[goalEnum.hardsaves.cityToll].exclusions.Add(goalEnum.extended.tollBenches);
            goals[goalEnum.hardsaves.colo].exclusions.Add(goalEnum.vanilla.colo);
            goals[goalEnum.hardsaves.colo].exclusions.Add(goalEnum.vanilla.coloZote);
            goals[goalEnum.hardsaves.colo].exclusions.Add(goalEnum.vanilla.paleLurker);
            goals[goalEnum.hardsaves.colo].exclusions.Add(goalEnum.vanilla.hotSprings);
            goals[goalEnum.hardsaves.colo].exclusions.Add(goalEnum.extended.tiso);
            goals[goalEnum.hardsaves.xroadsSpring].exclusions.Add(goalEnum.vanilla.hotSprings);
            goals[goalEnum.hardsaves.xroadsStag].exclusions.Add(goalEnum.extended.tiso);
            goals[goalEnum.hardsaves.deepnestSpring].exclusions.Add(goalEnum.vanilla.hotSprings);
            goals[goalEnum.hardsaves.dive].exclusions.Add(goalEnum.vanilla.dive);
            goals[goalEnum.hardsaves.dive].exclusions.Add(goalEnum.vanilla.soulMaster);
            goals[goalEnum.hardsaves.dive].exclusions.Add(goalEnum.grubs.citySanctumDive);
            goals[goalEnum.hardsaves.dive].exclusions.Add(goalEnum.relics.sealSanctum);
            goals[goalEnum.hardsaves.failedTram].exclusions.Add(goalEnum.extended.failedTramBench);
            goals[goalEnum.hardsaves.godseeker].exclusions.Add(goalEnum.extended.godtuner);
            goals[goalEnum.hardsaves.godseeker].exclusions.Add(goalEnum.extended.nothing);
            goals[goalEnum.hardsaves.greenpathStag].exclusions.Add(goalEnum.vanilla.thornsBaldurSpore);
            goals[goalEnum.hardsaves.greenpathStag].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.greenpathStag].exclusions.Add(goalEnum.grubs.greenpathMossKnight);
            goals[goalEnum.hardsaves.greenpathStag].exclusions.Add(goalEnum.relics.journalGreenpathStag);
            goals[goalEnum.hardsaves.greenpathToll].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.greenpathToll].exclusions.Add(goalEnum.extended.tollBenches);
            goals[goalEnum.hardsaves.greyMourner].exclusions.Add(goalEnum.vanilla.flowerQuest);
            goals[goalEnum.hardsaves.hiddenStag].exclusions.Add(goalEnum.vanilla.stagHidden);
            goals[goalEnum.hardsaves.hive].exclusions.Add(goalEnum.extended.grubsHive);
            goals[goalEnum.hardsaves.hive].exclusions.Add(goalEnum.grubs.hiveInternal);
            goals[goalEnum.hardsaves.brand].exclusions.Add(goalEnum.vanilla.hornet2);
            goals[goalEnum.hardsaves.brand].exclusions.Add(goalEnum.relics.journalCamp);
            goals[goalEnum.hardsaves.brand].exclusions.Add(goalEnum.hardsaves.camp);
            goals[goalEnum.hardsaves.kingsStag].exclusions.Add(goalEnum.relics.sealKings);
            goals[goalEnum.hardsaves.kingsStag].exclusions.Add(goalEnum.relics.journalAboveKings);
            goals[goalEnum.hardsaves.unn].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.legEater].exclusions.Add(goalEnum.vanilla.fragiles);
            goals[goalEnum.hardsaves.legEater].exclusions.Add(goalEnum.extended.fungalBenches);
            goals[goalEnum.hardsaves.legEater].exclusions.Add(goalEnum.extended.maceBugLeggyBench);
            goals[goalEnum.hardsaves.legEater].exclusions.Add(goalEnum.extended.visitShops);
            goals[goalEnum.hardsaves.lowerTram].exclusions.Add(goalEnum.vanilla.tram);
            goals[goalEnum.hardsaves.lurien].exclusions.Add(goalEnum.vanilla.lurien);
            goals[goalEnum.hardsaves.lurien].exclusions.Add(goalEnum.extended.telescope);
            goals[goalEnum.hardsaves.mantisVillage].exclusions.Add(goalEnum.vanilla.mantisLords);
            goals[goalEnum.hardsaves.mantisVillage].exclusions.Add(goalEnum.vanilla.longnail);
            goals[goalEnum.hardsaves.mantisVillage].exclusions.Add(goalEnum.extended.fungalBenches);
            goals[goalEnum.hardsaves.mantisVillage].exclusions.Add(goalEnum.relics.sealMantisLords);
            goals[goalEnum.hardsaves.mato].exclusions.Add(goalEnum.vanilla.cyclone);
            goals[goalEnum.hardsaves.monomon].exclusions.Add(goalEnum.vanilla.monomon);
            goals[goalEnum.hardsaves.oro].exclusions.Add(goalEnum.vanilla.dashSlash);
            goals[goalEnum.hardsaves.oro].exclusions.Add(goalEnum.extended.scarecrow);
            goals[goalEnum.hardsaves.oro].exclusions.Add(goalEnum.grubs.keOro);
            goals[goalEnum.hardsaves.pleasureHouse].exclusions.Add(goalEnum.vanilla.hotSprings);
            goals[goalEnum.hardsaves.pleasureHouse].exclusions.Add(goalEnum.extended.slashMillibelle);
            goals[goalEnum.hardsaves.pleasureHouse].exclusions.Add(goalEnum.extended.marissa);
            goals[goalEnum.hardsaves.pleasureHouse].exclusions.Add(goalEnum.relics.journalPleasureHouse);
            goals[goalEnum.hardsaves.qgCornifer].exclusions.Add(goalEnum.extended.mapsQGFog);
            goals[goalEnum.hardsaves.qgStag].exclusions.Add(goalEnum.vanilla.marmu);
            goals[goalEnum.hardsaves.qgStag].exclusions.Add(goalEnum.vanilla.stagQG);
            goals[goalEnum.hardsaves.qgStag].exclusions.Add(goalEnum.relics.sealQG);
            goals[goalEnum.hardsaves.qgToll].exclusions.Add(goalEnum.extended.tollBenches);
            goals[goalEnum.hardsaves.queensStationStag].exclusions.Add(goalEnum.vanilla.banker);
            goals[goalEnum.hardsaves.queensStationStag].exclusions.Add(goalEnum.extended.fungalBenches);
            goals[goalEnum.hardsaves.rgStag].exclusions.Add(goalEnum.vanilla.essence);
            goals[goalEnum.hardsaves.rgStag].exclusions.Add(goalEnum.vanilla.dnail);
            goals[goalEnum.hardsaves.rgStag].exclusions.Add(goalEnum.vanilla.wielder);
            goals[goalEnum.hardsaves.rgStag].exclusions.Add(goalEnum.vanilla.revek);
            goals[goalEnum.hardsaves.rgStag].exclusions.Add(goalEnum.vanilla.xero);
            goals[goalEnum.hardsaves.salubra].exclusions.Add(goalEnum.extended.visitShops);
            goals[goalEnum.hardsaves.shadeCloak].exclusions.Add(goalEnum.vanilla.shadeCloak);
            goals[goalEnum.hardsaves.shadeCloak].exclusions.Add(goalEnum.relics.eggShadeCloak);
            goals[goalEnum.hardsaves.sheo].exclusions.Add(goalEnum.vanilla.greatSlash);
            goals[goalEnum.hardsaves.sheo].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.stoneSanc].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.upperTram].exclusions.Add(goalEnum.vanilla.tram);
            goals[goalEnum.hardsaves.spire].exclusions.Add(goalEnum.vanilla.watchers);
            goals[goalEnum.hardsaves.spire].exclusions.Add(goalEnum.relics.sealSpire);
            goals[goalEnum.hardsaves.waterfall].exclusions.Add(goalEnum.extended.greenpathBenches);
            goals[goalEnum.hardsaves.waterways].exclusions.Add(goalEnum.grubs.waterwaysCenter);
        }

        private void setupGrubDict(Dictionary<string, BingoGoal> goals) {
            goals[goalEnum.grubs.xroadsWall].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.xroadsGuarded].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.xroadsSpike].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.xroadsVengefly].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.xroadsAcid].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.peakSpike].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakCrown].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakCrown].exclusions.Add(goalEnum.vanilla.cdash);
            goals[goalEnum.grubs.peakCdash].exclusions.Add(goalEnum.vanilla.cdash);
            goals[goalEnum.grubs.peakCdash].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakCrushers].exclusions.Add(goalEnum.vanilla.cdash);
            goals[goalEnum.grubs.peakCrushers].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakBottom].exclusions.Add(goalEnum.vanilla.cdash);
            goals[goalEnum.grubs.peakBottom].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakMound].exclusions.Add(goalEnum.vanilla.ddark);
            goals[goalEnum.grubs.peakMound].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakMimic].exclusions.Add(goalEnum.vanilla.mimics);
            goals[goalEnum.grubs.peakMimic].exclusions.Add(goalEnum.vanilla.grubsPeak);
            goals[goalEnum.grubs.peakMimic].exclusions.Add(goalEnum.vanilla.grimmUpgrade);
            goals[goalEnum.grubs.peakMimic].exclusions.Add(goalEnum.extended.dirtmouthElevator);
            goals[goalEnum.grubs.crypts].exclusions.Add(goalEnum.vanilla.flowerQuest);
            goals[goalEnum.grubs.crypts].exclusions.Add(goalEnum.extended.eaterCatcher);
            goals[goalEnum.grubs.crypts].exclusions.Add(goalEnum.relics.journalCrypts);
            goals[goalEnum.grubs.crypts].exclusions.Add(goalEnum.relics.sealCrypts);
            goals[goalEnum.grubs.citySanctumDive].exclusions.Add(goalEnum.vanilla.grubsCity);
            goals[goalEnum.grubs.citySanctumDive].exclusions.Add(goalEnum.vanilla.dive);
            goals[goalEnum.grubs.citySanctumDive].exclusions.Add(goalEnum.vanilla.soulMaster);
            goals[goalEnum.grubs.citySanctumDive].exclusions.Add(goalEnum.hardsaves.dive);
            goals[goalEnum.grubs.citySanctumDive].exclusions.Add(goalEnum.relics.sealSanctum);
            goals[goalEnum.grubs.cityBelowSanctum].exclusions.Add(goalEnum.vanilla.grubsCity);
            goals[goalEnum.grubs.cityBelowSanctum].exclusions.Add(goalEnum.relics.sealRafters);
            goals[goalEnum.grubs.cityBelowLove].exclusions.Add(goalEnum.vanilla.grubsCity);
            goals[goalEnum.grubs.cityGuard].exclusions.Add(goalEnum.vanilla.grubsCity);
            goals[goalEnum.grubs.citySpire].exclusions.Add(goalEnum.vanilla.grubsCity);
            goals[goalEnum.grubs.waterwaysCenter].exclusions.Add(goalEnum.vanilla.grubsWaterways);
            goals[goalEnum.grubs.waterwaysCenter].exclusions.Add(goalEnum.hardsaves.waterways);
            goals[goalEnum.grubs.waterwaysIsma].exclusions.Add(goalEnum.vanilla.grubsWaterways);
            goals[goalEnum.grubs.waterwaysHwurmp].exclusions.Add(goalEnum.vanilla.grubsWaterways);
            goals[goalEnum.grubs.fungalBouncy].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.fungalSpore].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.fungalSpore].exclusions.Add(goalEnum.vanilla.thornsBaldurSpore);
            goals[goalEnum.grubs.deepnestMimics].exclusions.Add(goalEnum.vanilla.mimics);
            goals[goalEnum.grubs.deepnestMimics].exclusions.Add(goalEnum.vanilla.grubsDeepnest);
            goals[goalEnum.grubs.deepnestNosk].exclusions.Add(goalEnum.vanilla.nosk);
            goals[goalEnum.grubs.deepnestNosk].exclusions.Add(goalEnum.vanilla.grubsDeepnest);
            goals[goalEnum.grubs.deepnestSpike].exclusions.Add(goalEnum.vanilla.grubsDeepnest);
            goals[goalEnum.grubs.deepnestDark].exclusions.Add(goalEnum.vanilla.grubsDeepnest);
            goals[goalEnum.grubs.deepnestDen].exclusions.Add(goalEnum.vanilla.herrah);
            goals[goalEnum.grubs.deepnestDen].exclusions.Add(goalEnum.vanilla.grubsDeepnest);
            goals[goalEnum.grubs.deepnestDen].exclusions.Add(goalEnum.vanilla.hornetHerrah);
            goals[goalEnum.grubs.archives].exclusions.Add(goalEnum.vanilla.grubsCross);
            goals[goalEnum.grubs.archives].exclusions.Add(goalEnum.hardsaves.archives);
            goals[goalEnum.grubs.qgBelowStag].exclusions.Add(goalEnum.vanilla.grubsQG);
            goals[goalEnum.grubs.qgWhiteLady].exclusions.Add(goalEnum.vanilla.grubsQG);
            goals[goalEnum.grubs.qgUpper].exclusions.Add(goalEnum.vanilla.grubsQG);
            goals[goalEnum.grubs.greenpathMossKnight].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.greenpathMossKnight].exclusions.Add(goalEnum.hardsaves.greenpathStag);
            goals[goalEnum.grubs.greenpathMossKnight].exclusions.Add(goalEnum.relics.journalGreenpathStag);
            goals[goalEnum.grubs.greenpathHunter].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.greenpathCornifer].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.greenpathVesselFrag].exclusions.Add(goalEnum.vanilla.grubsGreen);
            goals[goalEnum.grubs.greenpathVesselFrag].exclusions.Add(goalEnum.extended.greenpathRoot);
            goals[goalEnum.grubs.hiveExternal].exclusions.Add(goalEnum.extended.grubsHive);
            goals[goalEnum.grubs.hiveInternal].exclusions.Add(goalEnum.extended.grubsHive);
            goals[goalEnum.grubs.hiveInternal].exclusions.Add(goalEnum.hardsaves.hive);
            goals[goalEnum.grubs.basinDive].exclusions.Add(goalEnum.extended.grubsBasin);
            goals[goalEnum.grubs.basinDive].exclusions.Add(goalEnum.hardsaves.basinToll);
            goals[goalEnum.grubs.basinWings].exclusions.Add(goalEnum.extended.grubsBasin);
            goals[goalEnum.grubs.basinWings].exclusions.Add(goalEnum.hardsaves.basinToll);
            goals[goalEnum.grubs.collector].exclusions.Add(goalEnum.vanilla.collector);
            goals[goalEnum.grubs.keOro].exclusions.Add(goalEnum.vanilla.quickslash);
            goals[goalEnum.grubs.keOro].exclusions.Add(goalEnum.hardsaves.oro);
            goals[goalEnum.grubs.keCenter].exclusions.Add(goalEnum.relics.journalMarkothDive);
        }

        private void setupRelicDict(Dictionary<string, BingoGoal> goals) {
            goals[goalEnum.relics.journalAboveKings].exclusions.Add(goalEnum.hardsaves.kingsStag);
            goals[goalEnum.relics.journalAboveKings].exclusions.Add(goalEnum.relics.sealKings);
            goals[goalEnum.relics.journalBelowOgres].exclusions.Add(goalEnum.relics.sealSporgs);
            goals[goalEnum.relics.journalBelowStoneSanc].exclusions.Add(goalEnum.extended.stoneSancJournal);
            goals[goalEnum.relics.journalCamp].exclusions.Add(goalEnum.hardsaves.camp);
            goals[goalEnum.relics.journalCamp].exclusions.Add(goalEnum.vanilla.hornet2);
            goals[goalEnum.relics.journalCamp].exclusions.Add(goalEnum.hardsaves.brand);
            goals[goalEnum.relics.journalStorerooms].exclusions.Add(goalEnum.vanilla.grimmUpgrade);
            goals[goalEnum.relics.journalCrypts].exclusions.Add(goalEnum.relics.sealCrypts);
            goals[goalEnum.relics.journalCrypts].exclusions.Add(goalEnum.vanilla.flowerQuest);
            goals[goalEnum.relics.journalCrypts].exclusions.Add(goalEnum.extended.eaterCatcher);
            goals[goalEnum.relics.journalCrypts].exclusions.Add(goalEnum.grubs.crypts);
            goals[goalEnum.relics.journalPeakConga].exclusions.Add(goalEnum.relics.idolPeakCornifer);
            goals[goalEnum.relics.journalGreenpathStag].exclusions.Add(goalEnum.grubs.greenpathMossKnight);
            goals[goalEnum.relics.journalGreenpathStag].exclusions.Add(goalEnum.hardsaves.greenpathStag);
            goals[goalEnum.relics.journalCliffs].exclusions.Add(goalEnum.relics.idolCliffs);
            goals[goalEnum.relics.journalCliffs].exclusions.Add(goalEnum.vanilla.gwomb);
            goals[goalEnum.relics.journalCliffs].exclusions.Add(goalEnum.vanilla.grimmUpgrade);
            goals[goalEnum.relics.journalEdgeCornifer].exclusions.Add(goalEnum.relics.sealKings);
            goals[goalEnum.relics.journalMarkothDive].exclusions.Add(goalEnum.grubs.keCenter);
            goals[goalEnum.relics.journalPleasureHouse].exclusions.Add(goalEnum.extended.marissa);
            goals[goalEnum.relics.journalPleasureHouse].exclusions.Add(goalEnum.hardsaves.pleasureHouse);
            goals[goalEnum.relics.sealEssence].exclusions.Add(goalEnum.relics.idolGlade);
            goals[goalEnum.relics.sealEssence].exclusions.Add(goalEnum.vanilla.essence);
            goals[goalEnum.relics.sealEssence].exclusions.Add(goalEnum.vanilla.xero);
            goals[goalEnum.relics.seal23Grubs].exclusions.Add(goalEnum.vanilla.grubs20);
            goals[goalEnum.relics.sealDen].exclusions.Add(goalEnum.hardsaves.denHerrah);
            goals[goalEnum.relics.sealCrypts].exclusions.Add(goalEnum.relics.journalCrypts);
            goals[goalEnum.relics.sealCrypts].exclusions.Add(goalEnum.vanilla.flowerQuest);
            goals[goalEnum.relics.sealCrypts].exclusions.Add(goalEnum.extended.eaterCatcher);
            goals[goalEnum.relics.sealCrypts].exclusions.Add(goalEnum.grubs.crypts);
            goals[goalEnum.relics.sealKings].exclusions.Add(goalEnum.relics.journalEdgeCornifer);
            goals[goalEnum.relics.sealKings].exclusions.Add(goalEnum.vanilla.stagQueensKings);
            goals[goalEnum.relics.sealKings].exclusions.Add(goalEnum.hardsaves.kingsStag);
            goals[goalEnum.relics.sealKings].exclusions.Add(goalEnum.relics.journalAboveKings);
            goals[goalEnum.relics.sealMantisLords].exclusions.Add(goalEnum.vanilla.longnail);
            goals[goalEnum.relics.sealMantisLords].exclusions.Add(goalEnum.vanilla.mantisLords);
            goals[goalEnum.relics.sealMantisLords].exclusions.Add(goalEnum.hardsaves.mantisVillage);
            goals[goalEnum.relics.sealQueensStation].exclusions.Add(goalEnum.extended.willoh);
            goals[goalEnum.relics.sealRafters].exclusions.Add(goalEnum.grubs.cityBelowSanctum);
            goals[goalEnum.relics.sealRafters].exclusions.Add(goalEnum.hardsaves.cityGate);
            goals[goalEnum.relics.sealRafters].exclusions.Add(goalEnum.hardsaves.cityQuirrel);
            goals[goalEnum.relics.sealSanctum].exclusions.Add(goalEnum.vanilla.dive);
            goals[goalEnum.relics.sealSanctum].exclusions.Add(goalEnum.vanilla.soulMaster);
            goals[goalEnum.relics.sealSanctum].exclusions.Add(goalEnum.grubs.citySanctumDive);
            goals[goalEnum.relics.sealSanctum].exclusions.Add(goalEnum.hardsaves.dive);
            goals[goalEnum.relics.sealSporgs].exclusions.Add(goalEnum.relics.journalBelowOgres);
            goals[goalEnum.relics.sealSpire].exclusions.Add(goalEnum.vanilla.watchers);
            goals[goalEnum.relics.sealSpire].exclusions.Add(goalEnum.extended.telescope);
            goals[goalEnum.relics.sealSpire].exclusions.Add(goalEnum.hardsaves.spire);
            goals[goalEnum.relics.sealQG].exclusions.Add(goalEnum.vanilla.marmu);
            goals[goalEnum.relics.sealQG].exclusions.Add(goalEnum.vanilla.stagQG);
            goals[goalEnum.relics.sealQG].exclusions.Add(goalEnum.hardsaves.qgStag);
            goals[goalEnum.relics.idolPeakCornifer].exclusions.Add(goalEnum.relics.journalPeakConga);
            goals[goalEnum.relics.idolPeakCornifer].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolDeepnestZote].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolCliffs].exclusions.Add(goalEnum.relics.journalCliffs);
            goals[goalEnum.relics.idolCliffs].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolDungDefender].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolDungDefender].exclusions.Add(goalEnum.vanilla.ddefender);
            goals[goalEnum.relics.idolGreatHopper].exclusions.Add(goalEnum.relics.idolPaleLurker);
            goals[goalEnum.relics.idolGreatHopper].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolPaleLurker].exclusions.Add(goalEnum.relics.idolGreatHopper);
            goals[goalEnum.relics.idolPaleLurker].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolPaleLurker].exclusions.Add(goalEnum.vanilla.paleLurker);
            goals[goalEnum.relics.idolGlade].exclusions.Add(goalEnum.relics.sealEssence);
            goals[goalEnum.relics.idolGlade].exclusions.Add(goalEnum.vanilla.idols);
            goals[goalEnum.relics.idolGlade].exclusions.Add(goalEnum.vanilla.revek);
            goals[goalEnum.relics.eggEssence].exclusions.Add(goalEnum.vanilla.arcane);
            goals[goalEnum.relics.eggLifebloodCore].exclusions.Add(goalEnum.vanilla.arcane);
            goals[goalEnum.relics.eggLifebloodCore].exclusions.Add(goalEnum.vanilla.lifeblood);
            goals[goalEnum.relics.eggLifebloodCore].exclusions.Add(goalEnum.vanilla.mask1);
            goals[goalEnum.relics.eggLifebloodCore].exclusions.Add(goalEnum.extended.lifeblood10);
            goals[goalEnum.relics.eggShadeCloak].exclusions.Add(goalEnum.vanilla.arcane);
            goals[goalEnum.relics.eggShadeCloak].exclusions.Add(goalEnum.vanilla.shadeCloak);
            goals[goalEnum.relics.eggShadeCloak].exclusions.Add(goalEnum.vanilla.voidTendrils);
            goals[goalEnum.relics.eggShadeCloak].exclusions.Add(goalEnum.hardsaves.shadeCloak);
        }
    }
}