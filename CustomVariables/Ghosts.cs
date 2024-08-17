using Satchel;


namespace BingoGoalPack1.CustomVariables {
    internal static class Ghosts {
        private static string poggyVariableName = "killedPoggyGhost";
        private static string joniVariableName = "killedJoniGhost" ;
        private static string poggyRoomName = "Ruins_Elevator";
        private static string joniRoomName = "Cliffs_05";
        private static string objectName = "Dreamnail Hit";
        private static string fsmName = "ghost_npc_dreamnail";
        private static string killedStateName = "Send";

        public static void CreateGhostKilledTrigger(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.FsmName != fsmName)
                return;
            if(self.gameObject == null || self.gameObject.name!=objectName || self.gameObject.scene==null)
                return;
            string room = self.gameObject.scene.name;
            if(room == poggyRoomName) {
                self.AddCustomAction(killedStateName, () => BingoSync.Variables.UpdateBoolean(poggyVariableName, true));
            }
            if(room == joniRoomName) {
                self.AddCustomAction(killedStateName, () => BingoSync.Variables.UpdateBoolean(joniVariableName, true));
            }
        }
    }
}
