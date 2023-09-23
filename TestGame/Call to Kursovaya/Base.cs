using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Base : Building
    {
        public static new int gold_cost = 100;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 1;
        
        public Base(int _ally, Point point) : base(_ally, point)
        {
            gold_income = 0.15;
            sprite = new Bitmap(Resource1.Base);
            health = 3000;
            ally = _ally;
            entity_name = Unit_type.BASE;
            entity_class = Unit_type.BUILDING;
        }
        public override void Do(ActionEventArgs a)
        {
            //if (this.health <= 0 && ally == 2) Win();
            //if (this.health <= 0 && ally == 1) Defeat();
            base.Do(a);
        }

    }
}
