using Satchel;


namespace BingoGoalPack1.CustomVariables {
    internal static class ChestsExtension {
        private static string fsmName = "Chest Control";
        private static string gameObjectName = "Chest (3)";// nothing
        private static string sceneName = "GG_Waterways";  // junkpit
        private static string openStateName = "Opened";

        public static void CreateJunkpitChestTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName)
                return;
            if(self.gameObject == null || self.gameObject.name != gameObjectName)
                return;
            if(self.gameObject.scene == null || self.gameObject.scene.name != sceneName)
                return;
            self.AddCustomAction(openStateName, () => {
                string variableName = $"chestOpen_{GameManager.instance.GetSceneNameString()}_nothing";
                BingoSync.Variables.UpdateBoolean(variableName, true);
            });
        }

        public static void CreateMLordsChestTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName)
                return;
            if(self.gameObject.name == null || self.gameObject.scene == null)
                return;
            if(self.gameObject.name == "Mantis Chest (2)" && self.gameObject.scene.name == "Fungus2_31") {
                self.AddCustomAction(openStateName, () => {
                    string variableName = $"chestOpen_{GameManager.instance.GetSceneNameString()}_mantisSpecial";
                    BingoSync.Variables.UpdateBoolean(variableName, true);
                });
            }
        }
    }
}
