using Satchel;
using BingoSyncExtension;

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
                var alreadyHit = VariableProxy.GetBoolean(uniqueVariableName);
                if(alreadyHit)
                    return;
                VariableProxy.UpdateBoolean(variableName, true);
                var totemsHit = VariableProxy.GetInteger(variableName) + 1;
                VariableProxy.UpdateInteger(variableName, totemsHit);
            });
        }
    }
}
