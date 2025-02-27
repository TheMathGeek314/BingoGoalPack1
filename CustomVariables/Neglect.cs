namespace BingoGoalPack1.CustomVariables {
    internal static class Neglect {
        public static void CheckNeglectAchievement(On.GameManager.orig_AwardAchievement orig, GameManager self, string key) {
            orig(self, key);
            if(key == "NEGLECT") {
                BingoSync.Variables.UpdateBoolean("neglectedZote", true);
            }
        }
    }
}