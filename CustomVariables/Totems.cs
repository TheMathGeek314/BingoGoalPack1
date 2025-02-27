using Satchel;

namespace BingoGoalPack1.CustomVariables {
    internal static class Totems {
        private static string variableName = "soulTotemsHit";
        private static string fsmName = "soul_totem";
        private static string hitStateName = "Hit";

        public static void CreateSoulTotemTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName)
                return;
            if(self.gameObject == null)
                return;
            self.AddCustomAction(hitStateName, () => {
                string uniqueVariableName = $"hitSoulTotem_{self.gameObject.scene.name}_{self.gameObject.GetPath()}";
                var alreadyHit = BingoSync.Variables.GetBoolean(uniqueVariableName);
                if(alreadyHit)
                    return;
                BingoSync.Variables.UpdateBoolean(uniqueVariableName, true);
                var totemsHit = BingoSync.Variables.GetInteger(variableName) + 1;
                BingoSync.Variables.UpdateInteger(variableName, totemsHit);
            });
        }
    }
}
