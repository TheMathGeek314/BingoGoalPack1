using BingoSyncExtension;

namespace BingoGoalPack1.CustomVariables {
    class HallOfGodsTracker {
        internal static void CreateHogStatueTrigger(On.BossStatue.orig_SetPlaqueState orig, BossStatue self, BossStatue.Completion statueState, BossStatueTrophyPlaque plaque, string playerDataKey) {
            orig(self,statueState,plaque,playerDataKey);
            VariableProxy.UpdateBoolean($"{playerDataKey}_attuned", statueState.completedTier1);
            VariableProxy.UpdateBoolean($"{playerDataKey}_ascended", statueState.completedTier2);
            VariableProxy.UpdateBoolean($"{playerDataKey}_radiant", statueState.completedTier3);
        }
    }
}
