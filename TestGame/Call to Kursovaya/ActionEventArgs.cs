using System;

namespace Call_to_Kursovaya
{
	[Serializable]
	public class ActionEventArgs : EventArgs
	{
		public double gold, metal, oil, ai_gold, ai_metal, ai_oil;
		public int ally_base_x, enemy_base_x;
		public int size_x;
		public int size_y;
		public int mapsize_x;
		public int mapsize_y;
		
		public ActionEventArgs(double gold, double metal, double oil, double ai_gold, double ai_metal, double ai_oil, int ally_base_x, int enemy_base_x,
			int size_x, int size_y, int mapsize_x, int mapsize_y)
		{
			this.gold = gold;
			this.metal = metal;
			this.oil = oil;
			this.ai_gold = ai_gold;
			this.ai_metal = ai_metal;
			this.ai_oil = ai_oil;
			this.ally_base_x = ally_base_x;
			this.enemy_base_x = enemy_base_x;
			this.size_x = size_x;
			this.size_y = size_y;
			this.mapsize_x = mapsize_x;
			this.mapsize_y = mapsize_y;
		}
    }
}
