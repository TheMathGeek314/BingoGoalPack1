using GlobalEnums;
using BingoSyncExtension;

namespace BingoGoalPack1.CustomVariables {
    internal static class CoreShard {
        private static string variableName = "collectedCoreShard";
        public static bool CheckIfCoreShardWasCollected(string name, bool orig) {
            if(name != nameof(PlayerData.instance.heartPieceCollected) || !orig)
                return orig;

            var zone = GameManager.instance.sm.mapZone;
            if(zone != MapZone.DEEPNEST)
                return orig;

            VariableProxy.UpdateBoolean(variableName, true);
            return orig;
        }
    }
}
