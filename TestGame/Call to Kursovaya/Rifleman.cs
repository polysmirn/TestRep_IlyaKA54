using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Rifleman : Unit
    {
        public static new int gold_cost = 110;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 1;

        public Rifleman(int _ally, Point point) : base(_ally, point)
        {
            infantry_damage = 1.2;
            tank_damage = 0.3;
            navy_damage = 0.05;
            building_damage = 0.7;
            entity_class = Unit_type.INFANTRY;
            entity_name = Unit_type.RIFLEMAN;
            sprite = new Bitmap(Resource1.Rifleman);
            health = 70;
            damage = 5;
            range = 120;
            ally = _ally;
            speed = 1;
        }
    }
}
