using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class OilFactory : Building
    {
        public static new int gold_cost = 350;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 0;

        public OilFactory(int _ally, Point point) : base(_ally, point)
        {
            oil_income = 0.10;
            sprite = new Bitmap(Resource1.OilFactory);
            health = 200;
            ally = _ally;
            entity_name = Unit_type.OILFACTORY;
            entity_class = Unit_type.BUILDING;
        }
        public override void Do(ActionEventArgs a)
        {
            base.Do(a);
        }

    }
}
