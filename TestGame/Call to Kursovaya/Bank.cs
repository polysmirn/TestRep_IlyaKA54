using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Bank : Building
    {
        public static new int gold_cost = 100;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 1;

        public Bank(int _ally, Point point) : base(_ally, point)
        {
            gold_income = 0.35;
            sprite = new Bitmap(Resource1.Bank);
            health = 330;
            ally = _ally;
            entity_name = Unit_type.BANK;
            entity_class = Unit_type.BUILDING;
        }
        public override void Do(ActionEventArgs a)
        {
            base.Do(a);
        }

    }
}
