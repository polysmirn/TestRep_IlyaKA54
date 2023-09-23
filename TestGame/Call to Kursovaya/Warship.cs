using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Warship : Unit
    {
        public static new int gold_cost = 350;
        public static new int metal_cost = 100;
        public static new int oil_cost = 50;
        public static new int spawn_ground = 0;

        public Warship(int _ally, Point point) : base(_ally, point)
        {
            infantry_damage = 0.3;
            tank_damage = 1.1;
            navy_damage = 1.5;
            building_damage = 1.2;
            entity_class = Unit_type.NAVY;
            entity_name = Unit_type.WARSHIP;
            sprite = new Bitmap(Resource1.Warship);
            health = 400;
            damage = 80;
            range = 240;
            ally = _ally;
            speed = 1;
        }
    }
}

