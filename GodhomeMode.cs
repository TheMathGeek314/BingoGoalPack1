using System;
using System.Collections.Generic;
using System.Linq;
using BingoSyncExtension;

namespace BingoGoalPack1 {
    public class GodhomeMode: GameMode {
        string[] bosses = { "Gruz Mother", "Vengefly King", "Brooding Mawlek", "False Knight", "Failed Champion", "Hornet Protector", "Hornet Sentinel", "Massive Moss Charger", "Flukemarm", "Mantis Lords", "Sisters of Battle", "Oblobbles", "Hive Knight", "Broken Vessel", "Lost Kin", "Nosk", "Winged Nosk", "Collector", "God Tamer", "Crystal Guardian", "Enraged Guardian", "Uumuu", "Traitor Lord", "Grey Prince Zote", "Soul Warrior", "Soul Master", "Soul Tyrant", "Dung Defender", "White Defender", "Watcher Knights", "No Eyes", "Marmu", "Galien", "Markoth", "Xero", "Gorb", "Elder Hu", "Oro & Mato", "Paintmaster Sheo", "Nailsage Sly", "Pure Vessel", "Grimm", "Nightmare King" };
        string[] levels = { "Attuned ", "Ascended ", "Radiant " };

        public GodhomeMode() : base("Hall of Gods", new Dictionary<string, BingoGoal>()) { }

        public override string GenerateBoard() {
            Random r = new Random();
            List<BingoGoal> board = new List<BingoGoal>();
            List<string> newBosses = new List<string>(bosses);
            do {
                string level = levels[r.Next(6) == 1 ? 2 : (r.Next(2) == 1 ? 1 : 0)];
                string boss = newBosses.ElementAt(r.Next(newBosses.Count));
                newBosses.Remove(boss);
                board.Add(new BingoGoal(level + boss));
            } while(board.Count < 25);
            return Jsonify(board);
        }
    }
}
