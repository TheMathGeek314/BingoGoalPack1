using Satchel;


namespace BingoGoalPack1.CustomVariables {
    internal static class Sit {
        private static string fsmName = "Shop Region";
        private static string gameObjectName = "Sit Region";
        private static string stateName = "Sit";

        public static void CreateSitTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName)
                return;
            if(self.gameObject == null || self.gameObject.name != gameObjectName)
                return;
            self.AddCustomAction(stateName, () => {
                string variableName = $"satWith_{GameManager.instance.GetSceneNameString()}";
                BingoSync.Variables.UpdateBoolean(variableName, true);
            });
        }
    }
}
