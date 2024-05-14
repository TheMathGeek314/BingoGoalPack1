using Satchel;
using BingoSyncExtension;

namespace BingoGoalPack1.CustomVariables {
    internal static class Diary {
        private static string fsmName = "Conversation Control";
        private static string readStateName = "Box Up";
        private static string roomName = "Room_Bretta";
        private static string gameObjectName = "Diary";

        public static void MarkDiaryAsRead(string objectName) {
            string variableName = $"readDiary_{roomName}_{objectName}";
            VariableProxy.UpdateBoolean(variableName, true);
        }

        public static void CreateDiaryTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || fsmName != self.FsmName || self.gameObject == null || self.gameObject.name != gameObjectName)
                return;
            if(self.gameObject.scene.name != roomName)
                return;
            self.AddCustomAction(readStateName, () => {
                MarkDiaryAsRead(self.gameObject.name);
            });
        }
    }
}
