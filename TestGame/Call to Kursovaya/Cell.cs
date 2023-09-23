using System;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Cell
    {
        public int ground_type; //0 - вода, 1 - земля
        public int spawn_ability;//0 - никто, 1 - бот, 2 - игрок
        public bool occupied;
        
        public Cell(int ground_type, int spawn_ability)
        {
            this.ground_type = ground_type;
            this.spawn_ability = spawn_ability;
        }
        public Cell()
        {
            this.ground_type = 0;
            this.spawn_ability = 0;

        }
    }
}
