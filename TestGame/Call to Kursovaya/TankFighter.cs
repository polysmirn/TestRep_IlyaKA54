using System;
using System.Drawing;


namespace Call_to_Kursovaya
{
    [Serializable]
    public class TankFighter : Unit
    {
        public static new int gold_cost = 140;
        public static new int metal_cost = 0;
        public static new int oil_cost = 0;
        public static new int spawn_ground = 1;

        public TankFighter(int _ally, Point point) : base(_ally, point)
        {
            infantry_damage = 0.02;
            tank_damage = 2;
            navy_damage = 0.8;
            building_damage = 0.2;
            entity_class = Unit_type.INFANTRY;
            entity_name = Unit_type.TANKFIGHTER;
            sprite = new Bitmap(Resource1.Tankfighter);
            health = 40;
            damage = 30;
            range = 150;
            ally = _ally;
            speed = 0.9;
        }
    }
}
