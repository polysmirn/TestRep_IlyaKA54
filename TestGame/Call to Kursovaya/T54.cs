using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class T54 : Unit
    {
        public static new int gold_cost = 250;
        public static new int metal_cost = 75;
        public static new int oil_cost = 10;
        public static new int spawn_ground = 1;

        public T54(int _ally, Point point) : base(_ally, point)
        {
            infantry_damage = 0.7;
            tank_damage = 0.8;
            navy_damage = 0.3;
            building_damage = 0.6;
            entity_class = Unit_type.TANK;
            entity_name = Unit_type.T54;
            sprite  = new Bitmap(Resource1.Tank);
            health = 120;
            damage = 20;
            range = 140;
            ally = _ally;
            speed = 0.7;
        }
    }
}
