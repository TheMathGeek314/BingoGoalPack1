using GlobalEnums;

using Satchel;

namespace BingoGoalPack1.CustomVariables {
    internal static class GrubsExtension {
        private static string GetZoneGrubsVariableName(MapZone zone) {
            return $"grubsSaved_{zone}";
        }

        public static int CheckIfGrubWasSaved(string name, int orig) {
            if(name == nameof(PlayerData.grubsCollected)) {
                GrubSaved(GameManager.instance.sm.mapZone);
            }
            return orig;
        }

        private static void GrubSaved(MapZone zone) {
            if(zone.ToString() == "HIVE") {
                var variableName = GetZoneGrubsVariableName(zone);
                var grubsSaveOnZone = BingoSync.Variables.GetInteger(variableName) + 1;
                BingoSync.Variables.UpdateInteger(variableName, grubsSaveOnZone);
            }
        }

        internal static void SaveOneGrub(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self == null || self.gameObject == null || self.gameObject.scene == null)
                return;
            if(self.gameObject.name != "Grub" || self.FsmName != "Grub Control") {
                return;
            }
            self.AddCustomAction("Free", () => {
                string room = self.gameObject.scene.name;
                string variableName = $"grubSaved_single_{room}";
                //Collector special case
                if(room == "Ruins2_11")
                    BingoSync.Variables.Increment(variableName);
                else
                    BingoSync.Variables.UpdateBoolean(variableName, true);
            });
        }
    }
}
