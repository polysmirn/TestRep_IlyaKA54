using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Unit : Entity
    {

        protected int choice; //номер цели, которую юнит атакует (атакует он ближайший юнит)
        protected double choice_y;
        protected bool can_move; //0 - юнит не может продвинуться, 1 - может
        protected int damage = 0; // урон
        protected double speed = 0; // скорость перемещения
        protected int range; //дальность атаки
        protected double tank_damage, infantry_damage, navy_damage, building_damage;

        //public Action action;

        public Unit(int _ally, Point point ) : base(_ally, point)
        {

        }
       

        public override void Do(ActionEventArgs a)
        {
            if (this.health <= 0)
                DeleteIt();
            else
            if (this.y > a.size_x * (a.mapsize_y - 1))
            {
                for (int j = 0; j < objects.Count; j++)
                {
                    if (objects[j].entity_name == Unit_type.BASE & objects[j].ally == 2) objects[j].Damage(damage * 5);
                }
                DeleteIt();
            }
            else if (this.y < 0)
            {
                for (int j = 0; j < objects.Count; j++)
                {
                    if (objects[j].entity_name == Unit_type.BASE & objects[j].ally == 1) objects[j].Damage(damage * 5);
                }
                DeleteIt();
            }
            else
                if (!Attack(a)) Move(a);   
        }
        private bool Attack(ActionEventArgs a)
        {
            if (ally == 1) //поведение врага
            {
                choice = -1;
                choice_y = a.size_y * a.mapsize_y;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i] != this)
                        if (objects[i].ally == 2 && objects[i].x == this.x && objects[i].y <= (this.y + range) && objects[i].y > this.y)
                        {
                            if (objects[i].y < choice_y)
                            {
                                choice = i;
                                choice_y = objects[i].y;
                            }
                        }
                }

                if (choice != -1)
                {
                    switch (objects[choice].entity_class)
                    {
                        case (Unit_type.INFANTRY):
                            objects[choice].Damage((int)Math.Round((double)damage * infantry_damage));
                            break;
                        case (Unit_type.TANK):
                            objects[choice].Damage((int)Math.Round((double)damage * tank_damage));
                            break;
                        case (Unit_type.NAVY):
                            objects[choice].Damage((int)Math.Round((double)damage * navy_damage));
                            break;
                        case (Unit_type.BUILDING):
                            objects[choice].Damage((int)Math.Round((double)damage * building_damage));
                            break;
                    }
                    return true;
                }
                else return false;
            }
            else
            {
                choice = -1;
                choice_y = -1;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i] != this)
                        if (objects[i].ally == 1 && objects[i].x == this.x && objects[i].y >= (this.y - range) && objects[i].y < this.y)
                        {
                            if (objects[i].y > choice_y)
                            {
                                choice = i;
                                choice_y = objects[i].y;
                            }
                        }
                }

                if (choice != -1)
                {
                    switch (objects[choice].entity_class)
                    {
                        case (Unit_type.INFANTRY):
                            objects[choice].Damage((int)Math.Round((double)damage * infantry_damage));
                            break;
                        case (Unit_type.TANK):
                            objects[choice].Damage((int)Math.Round((double)damage * tank_damage));
                            break;
                        case (Unit_type.NAVY):
                            objects[choice].Damage((int)Math.Round((double)damage * navy_damage));
                            break;
                        case (Unit_type.BUILDING):
                            objects[choice].Damage((int)Math.Round((double)damage * building_damage));
                            break;
                    }
                    return true;
                }
                else return false;
            }
        }
        private void Move(ActionEventArgs a)
        {
            if (ally == 1)
            {
                can_move = true;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i] != this)
                        if (objects[i].x == this.x && objects[i].y < (this.y + a.size_y) && objects[i].y > this.y && objects[i].ally != this.ally)
                        {
                            can_move = false;
                        }
                }
                if (can_move == true) this.y += speed;
            }
            else
            {
                can_move = true;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i] != this)
                        if (objects[i].x == this.x && objects[i].y > (this.y + a.size_y) && objects[i].y < this.y && objects[i].ally != this.ally)
                        {
                            can_move = false;
                        }
                }
                if (can_move == true) this.y -= speed;
            }
        }  
        
    }
}
