using System;


namespace Call_to_Kursovaya
{
    public delegate void Spawn(Unit_type entity, int ally, int x, int y);
    [Serializable]
    public class AI
    {
        private int x, y, ground;
        public int bank_count, metalfactory_count, oilfactory_count;
        private Unit_type chosen_entity = Unit_type.NONE, chosen_class;
        [field: NonSerialized] public event Spawn SpawnEntity;
        public void ChangeDecision()
        {
            chosen_entity = Unit_type.NONE;
        }
        public void MakeDecision(Cell[,] cell, int enemybase_x, int mapsize_x, int mapsize_y, int size_y, int count)
        {
            Random rand = new Random();

            if (chosen_entity == Unit_type.NONE)
            {


                x = AI_defend(rand, enemybase_x, count);
                if (x != -1)
                {
                    int i = 0;
                    y = 0;
                    do
                    {
                        chosen_entity = AI_attack(rand);

                        if (cell[0, x].spawn_ability != 1 ||
                             (cell[0, x].ground_type != ground & ground != 2))
                            chosen_entity = Unit_type.NONE;
                        i++;
                    } while (chosen_entity == Unit_type.NONE && i <= 20);
                }
                else
                {
                    if (bank_count + metalfactory_count + oilfactory_count < 20)
                    {
                        chosen_entity = AI_build(rand);
                        chosen_class = Unit_type.BUILDING;
                    }
                    if (chosen_entity == Unit_type.NONE)
                    {
                        chosen_entity = AI_attack(rand);
                        chosen_class = Unit_type.UNIT;
                    }
                    if (chosen_entity != Unit_type.NONE)
                    {
                        Entity.CheckOccupacy(cell, mapsize_x, mapsize_y, size_y);
                        bool all_occupied = true;
                        foreach (Cell j in cell)
                        {
                            if (j.occupied == false && j.spawn_ability == 1) all_occupied = false;
                        }
                        if (all_occupied)
                        {
                            chosen_entity = Unit_type.NONE;
                        }
                        else
                        {
                            x = -1;
                            y = -1;
                            int i = 0;
                            while (x == -1 && y == -1)
                            {
                                y = rand.Next(0, mapsize_y);
                                x = rand.Next(0, mapsize_x);
                                if (cell[y, x].occupied == true || cell[y, x].spawn_ability != 1
                                    || (cell[y, x].ground_type != ground & ground != 2) ||
                                    (chosen_class == Unit_type.BUILDING && cell[y + 1, x].occupied == true))
                                {
                                    x = -1;
                                    y = -1;
                                }
                                i++;
                                if (i >= 20)
                                {
                                    chosen_entity = Unit_type.NONE;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            SpawnEntity?.Invoke(chosen_entity, 1, x, y);
        }
        private Unit_type AI_build(Random rand)
        {
            if (bank_count == 0 || rand.Next(0, 101) <= (oilfactory_count + metalfactory_count + 1) / (double)bank_count * 25)
            {
                ground = Bank.spawn_ground;
                return Unit_type.BANK;
            }
            else
            if ((bank_count > 1 && metalfactory_count == 0) || (rand.Next(0, 101) <= (bank_count + oilfactory_count) / (double)metalfactory_count * 10))
            {
                ground = MetalFactory.spawn_ground;
                return Unit_type.METALFACTORY;
            }
            else
                    if (bank_count > 4 & metalfactory_count > 2 & oilfactory_count == 0 || (rand.Next(0, 101) <= (bank_count + metalfactory_count) / (double)oilfactory_count * 5))
            {
                ground = OilFactory.spawn_ground;
                return Unit_type.OILFACTORY;
            }
            else return Unit_type.NONE;
        }

        private int AI_defend(Random rand, int enemy_base_x, int count)
        {
            int danger = 0;
            int dangerMax = 0;
            int chosen_i = -1;
            int[] units = new int[2], buildings = new int[2];
            units = Entity.GetUnits(enemy_base_x);
            danger = units[0] * 100 - units[1] * 50;
            if (rand.Next(0, 101) <= danger)
                return enemy_base_x;

            for (int i = 0; i < count; i++)
            {
                buildings = Entity.GetBuildings(i);
                units = Entity.GetUnits(i);
                danger = buildings[1] * (units[0] * 30 - units[1] * 15);
                if (danger > dangerMax)
                {
                    dangerMax = danger;
                    chosen_i = i;
                }
            }
            if (rand.Next(0, 101) < dangerMax) return chosen_i;
            else return -1;
        }
        private Unit_type AI_attack(Random rand)
        {
            Unit_type chosen_entity = Unit_type.NONE;
            while (chosen_entity == Unit_type.NONE)
            {
                int j = rand.Next(0, 5);
                switch (j)
                {
                    case (0):
                        ground = Rifleman.spawn_ground;
                        chosen_entity = Unit_type.RIFLEMAN;
                        break;
                    case (1):
                        ground = TankFighter.spawn_ground;
                        chosen_entity = Unit_type.TANKFIGHTER;
                        break;
                    case (2):
                        if ((metalfactory_count > 0 && T54.metal_cost > 0) && (oilfactory_count > 0 && T54.oil_cost > 0))
                        {
                            ground = T54.spawn_ground;
                            return Unit_type.T54;
                        }
                        else chosen_entity = Unit_type.NONE;
                        break;
                    case (3):
                        if ((metalfactory_count > 0 && Warship.metal_cost > 0) && (oilfactory_count > 0 && Warship.oil_cost > 0))
                        {
                            ground = Warship.spawn_ground;
                            return Unit_type.WARSHIP;
                        }
                        else chosen_entity = Unit_type.NONE;
                        break;
                    default:
                        chosen_entity = Unit_type.NONE;
                        break;
                }
            }
            return chosen_entity;
        }
        public void PlaceBase(Cell[,] cell, int mapsize_x)
        {
            Random rand = new Random();
            int ai_x = -1;
            int ai_y = -1;
            while (ai_x == -1 & ai_y == -1)
            {
                ai_y = 0;
                ai_x = rand.Next(0, mapsize_x);
                if (cell[ai_y, ai_x].occupied == true || cell[ai_y, ai_x].spawn_ability != 1
                    || cell[ai_y, ai_x].ground_type != 1)
                {
                    ai_x = -1;
                    ai_y = -1;
                }
            }
            SpawnEntity?.Invoke(Unit_type.BASE, 1, ai_x, ai_y);
        }
    }
}
