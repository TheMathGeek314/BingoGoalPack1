using Satchel;

namespace BingoGoalPack1.CustomVariables {
    internal static class Arenas {
        public static void CreateArenaTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName!="Battle Control")
                return;
            if(self.gameObject == null || self.gameObject.name != "Battle Scene")
                return;
            try {

                self.AddCustomAction("End", () => {
                    var room = self.gameObject.scene.name;
                    var variableName = $"arenaBeat_{room}";
                    BingoSync.Variables.UpdateBoolean(variableName, true);
                });
            }
            catch {}
        }
    }
}
