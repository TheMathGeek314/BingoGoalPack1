using Satchel;
using BingoSyncExtension;

namespace BingoGoalPack1.CustomVariables {
    internal static class DreamNailMace {
        private static string fsmName = "enemy_dreamnail_reaction";//or Mace Control
        private static string hitStateName = "Send Msg";

        public static void CreateMaceTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName || self.gameObject == null)
                return;
            if(self.gameObject.name != "Mace Head Bug(Clone)")
                return;
            self.AddCustomAction(hitStateName, () => {
                VariableProxy.UpdateBoolean("dreamNailedMaceBug", true);
            });
        }
    }
}
