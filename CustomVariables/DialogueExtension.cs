using BingoSyncExtension;

namespace BingoGoalPack1.CustomVariables {
    internal static class DialogueExtension {
        public static void StartConversation(On.DialogueBox.orig_StartConversation orig, DialogueBox self, string convName, string sheetName) {
            orig(self, convName, sheetName);
            // Divine with Crest
            if(convName == "DIVINE_DUNG_CHARM") {
                VariableProxy.UpdateBoolean("metDivineWithCrest", true);
            }
            // Quirrel
            if(sheetName == "Quirrel") {
                var scene = GameManager.instance.GetSceneNameString();
                var variableName = $"quirrel_{scene}";
                VariableProxy.UpdateBoolean(variableName, true);
            }
        }
    }
}
