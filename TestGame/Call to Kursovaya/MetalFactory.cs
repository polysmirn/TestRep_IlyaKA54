using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class MetalFactory : Building
    {
        public static new int gold_cost = 250;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 2;

        public MetalFactory(int _ally, Point point, int ground) : base(_ally, point)
        {
            if (ground == 1)
            {
                metal_income = 0.10;
                sprite = new Bitmap(Resource1.MetalFactory_Ground);
                health = 400;
                //gold_cost = 250;
            }
            else
            {
                health = 250;
                sprite = new Bitmap(Resource1.MetalFactory_Water);
                metal_income = 0.20;
                //gold_cost = 400;
            }

            ally = _ally;
            entity_name = Unit_type.METALFACTORY;
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
