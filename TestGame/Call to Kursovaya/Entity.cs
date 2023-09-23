using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Call_to_Kursovaya
{
    public delegate void Delete(Entity entity);
    [Serializable]
    public class Entity
    {

        [field: NonSerialized] public event Delete DeleteMe;
        public int x { get; protected set; } // координата по х (от 0 до 11)
        public double y { get; protected set; } // координата по у (от 0 до 839)
        protected int health = 1; // здоровье объекта
        public int ally { get; protected set; }  //1 - враг, 2 - наш 
        public static int gold_cost; //сколько золота стоит 
        public static int metal_cost; //сколько металла стоит
        public static int oil_cost; //сколько нефти стоит
        public Unit_type entity_class { get; protected set; }
        public Unit_type entity_name { get; protected set; }
        protected Image sprite;
        protected static List<Entity> objects = new List<Entity>();
        public int spawn_ground { get; protected set; } //0 - вода; 1 - земля; 2 - оба


        public virtual void Do(ActionEventArgs a)
        {
            if (this.health <= 0) DeleteIt();
        }

        protected virtual void DeleteIt()
        {
            objects.Remove(this);
            DeleteMe(this);
        }

        public Entity(int _ally, Point point)
        {
            ally = _ally;
            x = point.X;
            y = point.Y;
            objects.Add(this);
        }



        public void Damage(int damage)
        {
            health -= damage;
        }

        public void Draw(int size_x, int size_y, double resizeY, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.sprite, this.x * size_x, Form1.Round((int)this.y, resizeY), size_x, size_y);
        }
        public static void ChangeObjects(List<Entity> entities)
        {
            objects.Clear();
            objects.AddRange(entities);
        }
        public static int[] GetUnits(int x)
        {
            int ally_count = 0, enemies_count = 0;
            foreach (Entity i in objects)
            {
                if (i.ally == 2 && i.entity_class != Unit_type.BUILDING && i.x == x) ally_count += 1;
                if (i.ally == 1 && i.entity_class != Unit_type.BUILDING && i.x == x) enemies_count += 1;
            }
            return new int[2] { ally_count, enemies_count };
        }

        public static int[] GetBuildings(int x)
        {
            int ally_count = 0, enemies_count = 0;
            foreach (Entity i in objects)
            {
                if (i.ally == 2 && i.entity_class == Unit_type.BUILDING && i.x == x) ally_count += 1;
                if (i.ally == 1 && i.entity_class == Unit_type.BUILDING && i.x == x) enemies_count += 1;
            }
            return new int[2] { ally_count, enemies_count };
        }
        public static void CheckOccupacy(Cell[,] cells, int mapsize_x, int mapsize_y, int size_y)
        {
            foreach (Cell i in cells)
            {
                i.occupied = false;
            }
            foreach (Entity i in objects)
            {
                if (i.x >= 0 & i.x < mapsize_x & i.y >= 0 & i.y < size_y * mapsize_y)
                    cells[(int)i.y / size_y, i.x].occupied = true;
            }
        }

    }
}
