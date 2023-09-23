using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Call_to_Kursovaya
{
    [Serializable]
    public partial class Form1 : Form
    {
        private List<Label> labels;
        private List<Button> buttons; 

        private Resizing resizing;


        [Serializable] private delegate void Action(ActionEventArgs a);
        private delegate void Draw(int size_x, int size_y, double resizeY, PaintEventArgs e);

        event Action Step;
        event Draw Drawing;
        private static int size_x = 60;
        private static int size_y = 60;
        private double coefWidth;
        private double coefHeight;

        private int defaultX, defaultY;

        private double resizeX = 1;
        private double resizeY = 1;
        private int mapsize_x;
        private int mapsize_y;
        private Point cursor_coords;

        private GameArgs game;
        private ActionEventArgs a;

        private bool button_pressed;
        private Unit_type selected_unit;

        private bool paused, paused1;

        private AI ai;

        private Point point;

        private int[,] map;
        private int[,] spawn;
        private void button9_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.BANK;
            button_pressed = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void button7_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.METALFACTORY;
            button_pressed = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.OILFACTORY;
            button_pressed = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.WARSHIP;
            button_pressed = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            paused1 = paused;
            paused = true;
            BinaryFormatter formatter = new BinaryFormatter();

            Stream fs;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();



            saveFileDialog1.Filter = "dat files (*.dat)|*.dat";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((fs = saveFileDialog1.OpenFile()) != null)
                    {
                    game.paused = paused1;
                    formatter.Serialize(fs, game);
                    MessageBox.Show(
                    "Игра сохранена",
                    "Сообщение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                    }

                fs.Close();
                if (fs != null) fs = null;
            }


            paused = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            paused1 = paused;
            paused = true;
            BinaryFormatter formatter = new BinaryFormatter();
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "dat files (*.dat)|*.dat";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    Drawing = null;
                    Step = null;
                    game = (GameArgs)formatter.Deserialize(fileStream);

                    paused = game.paused;
                    a = game.args;

                    for (int i = 0; i < game.objects.Count; i++)
                    {
                        game.objects[i].DeleteMe += DeleteEntity;
                        Drawing += new Draw(game.objects[i].Draw);
                        Step += new Action(game.objects[i].Do);
                    }
                    Entity.ChangeObjects(game.objects);
                    button1.Enabled = false;
                    for (int i = 1; i < buttons.Count; i++)
                    {
                        buttons[i].Enabled = true;
                    }
                    ai.SpawnEntity += SpawnEntity;
                    
                }
                else paused = paused1;
            }
        }


        public Form1()
        {
            InitializeComponent();
            this.MinimumSize = new Size(450, 600 + aboutToolStripMenuItem.Size.Height);
            Size size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            coefWidth = (size.Width / (double)1920);
            coefHeight = (size.Height / (double)1080);
            resizeX = 1;
            resizeY = 1;
            defaultX = this.Width;
            defaultY = this.Height;
            labels = new List<Label> { label1 };
            buttons = new List<Button> { button1, button9, button7, button8, button2, button3, button6, button11, button4, button5, button10 };
            resizing = new Resizing(labels, buttons);
            CreateMap();
            ResizeForm();
            this.Text = "Call to Kursovaya";

            timer1.Interval = 30;
            timer1.Tick += new EventHandler(Update);
            timer1.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            if (!paused)
            {
                Step?.Invoke(a);

                ai.MakeDecision(game.cell, a.enemy_base_x, mapsize_x, mapsize_y, size_y, game.objects.Count);

                label1.Text = "Золото: " + Math.Round(a.gold) + "\n" + "Металл: " + Math.Round(a.metal) + "\n" + "Топливо: " + Math.Round(a.oil);
            }
            Invalidate();
            
        }



        private void SpawnEntity(Unit_type type, int ally, int x, int y)
        {
            Point point = new Point(x, y * size_y);
            switch (type)
            {
                case (Unit_type.T54):
                    if (a.ai_gold >= T54.gold_cost && a.ai_metal >= T54.metal_cost && a.ai_oil >= T54.oil_cost && ally == 1)
                    {
                        CreateEntity(new T54(1, point));
                        a.ai_gold -= T54.gold_cost;
                        a.ai_metal -= T54.metal_cost;
                        a.ai_oil -= T54.oil_cost;
                        ai.ChangeDecision();
                    }
                    else if (a.gold >= T54.gold_cost && a.metal >= T54.metal_cost && a.oil >= T54.oil_cost && ally == 2 && T54.spawn_ground == game.cell[y, x].ground_type) 
                    {
                        CreateEntity(new T54(2, point));
                        a.gold -= T54.gold_cost;
                        a.metal -= T54.metal_cost;
                        a.oil -= T54.oil_cost;
                    }
                    break;
                case (Unit_type.RIFLEMAN):
                    if (a.ai_gold >= Rifleman.gold_cost & a.ai_metal >= Rifleman.metal_cost & a.ai_oil >= Rifleman.oil_cost && ally == 1)
                    {
                        CreateEntity(new Rifleman(1, point));
                        a.ai_gold -= Rifleman.gold_cost;
                        a.ai_metal -= Rifleman.metal_cost;
                        a.ai_oil -= Rifleman.oil_cost;
                        ai.ChangeDecision();
                    }
                    else if (a.gold >= Rifleman.gold_cost & a.metal >= Rifleman.metal_cost & a.oil >= Rifleman.oil_cost && ally == 2 && Rifleman.spawn_ground == game.cell[y, x].ground_type)
                    {
                        CreateEntity(new Rifleman(2, point));
                        a.gold -= Rifleman.gold_cost;
                        a.metal -= Rifleman.metal_cost;
                        a.oil -= Rifleman.oil_cost;
                    }
                    break;
                case (Unit_type.TANKFIGHTER):
                    if (a.ai_gold >= TankFighter.gold_cost & a.ai_metal >= TankFighter.metal_cost & a.ai_oil >= TankFighter.oil_cost && ally == 1)
                        {
                            CreateEntity(new TankFighter(1, point));
                            a.ai_gold -= TankFighter.gold_cost;
                            a.ai_metal -= TankFighter.metal_cost;
                            a.ai_oil -= TankFighter.oil_cost;
                            ai.ChangeDecision();
                        }
                        else if (a.gold >= TankFighter.gold_cost & a.metal >= TankFighter.metal_cost & a.oil >= TankFighter.oil_cost && ally == 2 && TankFighter.spawn_ground == game.cell[y, x].ground_type)
                        {
                            CreateEntity(new TankFighter(2, point));
                            a.gold -= TankFighter.gold_cost;
                            a.metal -= TankFighter.metal_cost;
                            a.oil -= TankFighter.oil_cost;
                        }
                    break;
                case (Unit_type.WARSHIP):
                    if (a.ai_gold >= Warship.gold_cost && a.ai_metal >= Warship.metal_cost && a.ai_oil >= Warship.oil_cost && ally == 1)
                    {
                        CreateEntity(new Warship(1, point));
                        a.ai_gold -= Warship.gold_cost;
                        a.ai_metal -= Warship.metal_cost;
                        a.ai_oil -= Warship.oil_cost;
                        ai.ChangeDecision();
                    }
                    else if (a.gold >= Warship.gold_cost && a.metal >= Warship.metal_cost && a.oil >= Warship.oil_cost && ally == 2 && Warship.spawn_ground == game.cell[y, x].ground_type)
                    {
                        CreateEntity(new Warship(2, point));
                        a.gold -= Warship.gold_cost;
                        a.metal -= Warship.metal_cost;
                        a.oil -= Warship.oil_cost;
                    }
                    break;
                case Unit_type.BASE:
                    if (a.ai_gold >= Base.gold_cost && a.ai_metal >= Base.metal_cost && a.ai_oil >= Base.oil_cost && Base.spawn_ground == game.cell[y, x].ground_type && ally == 1)
                    {
                        CreateEntity(new Base(1, point));
                        a.ai_gold -= Base.gold_cost;
                        a.ai_metal -= Base.metal_cost;
                        a.ai_oil -= Base.oil_cost;
                        a.enemy_base_x = x;
                    }
                    else if (a.gold >= Base.gold_cost && a.metal >= Base.metal_cost && a.oil >= Base.oil_cost && Base.spawn_ground == game.cell[y, x].ground_type && ally == 2)
                    {
                        CreateEntity(new Base(2, point));
                        a.gold -= Base.gold_cost;
                        a.metal -= Base.metal_cost;
                        a.oil -= Base.oil_cost;
                        button1.Enabled = false;
                        for (int i = 1; i < buttons.Count; i++)
                        {
                            buttons[i].Enabled = true;
                        }
                        paused = false;
                    }
                    break;
                case (Unit_type.BANK):
                    if (a.ai_gold >= Bank.gold_cost && a.ai_metal >= Bank.metal_cost && a.ai_oil >= Bank.oil_cost && ally == 1)
                    {
                        CreateEntity(new Bank(1, point));
                        a.ai_gold -= Bank.gold_cost;
                        a.ai_metal -= Bank.metal_cost;
                        a.ai_oil -= Bank.oil_cost;
                        ai.ChangeDecision();
                        ai.bank_count += 1;
                    }
                    else if (a.gold >= Bank.gold_cost && a.metal >= Bank.metal_cost && a.oil >= Bank.oil_cost && ally == 2 && Bank.spawn_ground == game.cell[y, x].ground_type)
                    {
                        CreateEntity(new Bank(2, point));
                        a.gold -= Bank.gold_cost;
                        a.metal -= Bank.metal_cost;
                        a.oil -= Bank.oil_cost;
                    }
                    break;
                case (Unit_type.METALFACTORY):
                    if (a.ai_gold >= MetalFactory.gold_cost && a.ai_metal >= MetalFactory.metal_cost && a.ai_oil >= MetalFactory.oil_cost && ally == 1)
                    {
                        CreateEntity(new MetalFactory(1, point, game.cell[y, x].ground_type));
                        a.ai_gold -= MetalFactory.gold_cost;
                        a.ai_metal -= MetalFactory.metal_cost;
                        a.ai_oil -= MetalFactory.oil_cost;
                        ai.ChangeDecision();
                        ai.metalfactory_count += 1;
                    }
                    else if (a.gold >= MetalFactory.gold_cost && a.metal >= MetalFactory.metal_cost && a.oil >= MetalFactory.oil_cost && ally == 2)
                    {
                        CreateEntity(new MetalFactory(2, point, game.cell[y, x].ground_type));
                        a.gold -= MetalFactory.gold_cost;
                        a.metal -= MetalFactory.metal_cost;
                        a.oil -= MetalFactory.oil_cost;
                    }
                    break;
                case (Unit_type.OILFACTORY):
                    if (a.ai_gold >= OilFactory.gold_cost && a.ai_metal >= OilFactory.metal_cost && a.ai_oil >= OilFactory.oil_cost && ally == 1)
                    {
                        CreateEntity(new OilFactory(1, point));
                        a.ai_gold -= OilFactory.gold_cost;
                        a.ai_metal -= OilFactory.metal_cost;
                        a.ai_oil -= OilFactory.oil_cost;
                        ai.ChangeDecision();
                        ai.oilfactory_count += 1;
                    }
                    else if (a.gold >= OilFactory.gold_cost && a.metal >= OilFactory.metal_cost && a.oil >= OilFactory.oil_cost && ally == 2 && OilFactory.spawn_ground == game.cell[y, x].ground_type)
                    {
                        CreateEntity(new OilFactory(2, point));
                        a.gold -= OilFactory.gold_cost;
                        a.metal -= OilFactory.metal_cost;
                        a.oil -= OilFactory.oil_cost;
                    }
                    break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.BASE;
            button_pressed = true;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (button_pressed && e.Button == MouseButtons.Left)
            { 
                //cursor_coords = this.PointToClient(Cursor.Position);
                //point = GetPoint(cursor_coords);
                Entity.CheckOccupacy(game.cell, mapsize_x, mapsize_y, size_y);
                if (point.X != -1)
                    if (game.cell[point.Y, point.X].spawn_ability == 2 && game.cell[point.Y, point.X].occupied == false)
                    {
                        SpawnEntity(selected_unit, 2, point.X, point.Y);
                        
                    }

                button_pressed = false;
            }
            else if (button_pressed && e.Button == MouseButtons.Right)
            {
                button_pressed = false;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.T54;
            button_pressed = true;
        }

        private void CreateEntity(Entity entity)
        {
            if (entity.ally != 1) ai.ChangeDecision();
            game.objects.Add(entity);
            Step += new Action(entity.Do);
            Drawing += new Draw(entity.Draw);
            entity.DeleteMe += DeleteEntity;
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Суть данной игры довольно проста. Ваша задача - уничтожить вражескую базу. Лишь учичтожив её вы сможете победить. А проиграете вы, если то же самое враг сделает с вашей базой. Для победы вам необходимо нанять армию, которая будет способна разгромить вражескую базу. Уничтожить её можно двумя способами: первый довольно очевижен : подвести к ней свою армию. А второй более экзотичен: когда ваши бойцы достигают конца карты, то они исчезают и наносят вражеской базе урон, во много превосходящий их собственный. Ига начинается после того, как вы разместите базу. Для найма бойцов у вас есть 3 основых типа ресурсов: золото, металл и нефть. Для их получения вам нужно строить соответствующие здания (банки, заводы и нефтезаводы), которые будут приносить пассивный доход. При этом ваша база так же добывает золото, пусть и в небольших количествах. Здания и бойцов вы можете создавать при нажатии на соответствующую кнопку и выборе места их размещения на поле. При этом их размещать нужно на соответствующем типе повехности. Танк, стрелка, базу и банк вы можете разместить только на земле. Эсминец и нефтезавод на воде, а вот завод можно размещать на обоих видах поверхности (однако завод, построенный на воде, приносит меньший доход). У каждого бойца есть свои сильные и слабые стороны. Так, боец недорого стоит и быстро перемещается, однако имеет малый урон и наносит уменьшенный урон танкам, танк имеет большой урон и количество здоровья, хорош против зданий и пехоты, однако медленно передвигается. Корабль имеет большой урон, однако не может плыть по земле (что печально) и дорого стоит. Однако, он имеет колоссальный урон и, как упоминалось ранее, когда он достигает конца карты, наносит вражеской базе повышенный урон, что в купе с его имеющимся делает его крайне эффективным. Всего нескольких эсминцев, достигших конца карты будет достаточно, для уничтожения базы. Подробнее о характеристиках юнитов вы можете узнать во вкладке 'Глоссарий'.",
            "Правила",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Приложение разработал Евгений Бобров, группа 2-42В.\n" +
            "Bebrov Co and Bebrov Inc (c)",
            "Автор",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }

        private void глоссарийToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private string GetGround(int i)
        {
            switch (i)
            {
                case (0):
                    return "Вода";
                case (1):
                    return "Суша";
                default:
                    return "Вода и суша";
            }
        }

        private void постройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            ShowData("База", Base.gold_cost, Base.metal_cost, Base.oil_cost, Base.spawn_ground) +
            ShowData("Банк", Bank.gold_cost, Bank.metal_cost, Bank.oil_cost, Bank.spawn_ground) +
            ShowData("Завод", MetalFactory.gold_cost, MetalFactory.metal_cost, MetalFactory.oil_cost, MetalFactory.spawn_ground) +
            ShowData("Нефтезавод", OilFactory.gold_cost, OilFactory.metal_cost, OilFactory.oil_cost, OilFactory.spawn_ground),
            "О постройках",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information); ;
        }

        private string ShowData(string name, int gold_cost, int metal_cost, int oil_cost, int spawn_ground)
        {
            return name + "\n" +
            "Стоимость найма: " +
            "Золото: " + gold_cost + "\n" +
            "Металл: " + metal_cost + "\n" +
            "Нефть: " + oil_cost + "\n" +
            "Поверхность для создания: " + GetGround(spawn_ground) + "\n\n";
        }
       

        private void button10_Click(object sender, EventArgs e)
        {
            if (paused)
            {
                paused = false;
                button10.Text = "Пауза";
            }
            else
            {
                paused = true;
                button10.Text = "Продолжить";
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.TANKFIGHTER;
            button_pressed = true;
        }

        private void армияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            ShowData("Стрелок", Rifleman.gold_cost, Rifleman.metal_cost, Rifleman.oil_cost, Rifleman.spawn_ground) +
            ShowData("T54", T54.gold_cost, T54.metal_cost, T54.oil_cost, T54.spawn_ground) +
            ShowData("Эсминец", Warship.gold_cost, Warship.metal_cost, Warship.oil_cost, Warship.spawn_ground),
            "Об армии",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selected_unit = Unit_type.RIFLEMAN;
            button_pressed = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (defaultX != 0 && defaultY != 0)
            {
                resizeX = (double)this.Width / (double)defaultX;
                resizeY = (double)this.Height / (double)defaultY;
                ResizeForm();
            }
        }

        private void ResizeForm()
        {
            labels[0].Location = Resizing.Resize(resizeX, aboutToolStripMenuItem.Size.Height + 10, 0);
            labels[0].Font = Resizing.Resize(labels[0].Font, resizeX, resizeY, 0);
            labels[0].Size = Resizing.Resize(resizeX, resizeY, 0);

            for (int i = 1; i < labels.Count; i++)
            {
                labels[i].Location = Resizing.Resize(resizeX, labels[i - 1].Location.Y + labels[i - 1].Height, i);
                labels[i].Font = Resizing.Resize(labels[i].Font, resizeX, resizeY, i);
                labels[i].Size = Resizing.Resize(resizeX, resizeY, i);
            }

            buttons[0].Location = Resizing.Resize(resizeX, labels[^1].Location.Y + labels[^1].Height, 0);
            buttons[0].Font = Resizing.Resize(buttons[0].Font, resizeX, resizeY, labels.Count);
            buttons[0].Size = Resizing.Resize(resizeX, resizeY, labels.Count);
            for (int i = 1; i < buttons.Count; i++)
            {
                buttons[i].Location = Resizing.Resize(resizeX, buttons[i - 1].Location.Y + buttons[i - 1].Height, i + labels.Count);
                buttons[i].Font = Resizing.Resize(buttons[i].Font, resizeX, resizeY, i + labels.Count);
                buttons[i].Size = Resizing.Resize(resizeX, resizeY, i + labels.Count);
            }
        }
        public void DeleteEntity(Entity entity)
        {
            ai.ChangeDecision();
            Step -= entity.Do;
            Drawing -= entity.Draw;
            game.objects.Remove(entity);
            entity.DeleteMe -= DeleteEntity;
            if (entity.entity_name == Unit_type.BASE && entity.ally == 2) DefeatEventHandler();
            if (entity.ally == 1)
                switch (entity.entity_name)
                {
                    case (Unit_type.BANK):
                        ai.bank_count -= 1;
                        break;
                    case (Unit_type.METALFACTORY):
                        ai.metalfactory_count -= 1;
                        break;
                    case (Unit_type.OILFACTORY):
                        ai.oilfactory_count -= 1;
                        break;
                    case (Unit_type.BASE):
                        WinEventHandler();
                        break;
                }
        }

        public void WinEventHandler()
        {
            paused = true;
            if (MessageBox.Show(
        "Вы победили!\n Хотите начать заново?",
        "Сообщение",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Information) == DialogResult.Yes)
            {
                CreateMap();
            }
            else this.Close();

        }

        public void DefeatEventHandler()
        {
            paused = true;
            if (MessageBox.Show(
        "Вы проиграли!\n Хотите начать заново?",
        "Сообщение",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Information) == DialogResult.Yes)
            {
                CreateMap();
            }
            else this.Close();

        }

        private void CreateMap()
        {
            mapsize_x = 11;
            mapsize_y = 14;

            ai = new AI();
            a = new ActionEventArgs(150, 0, 0, 150, 0, 0, 0, 0, size_x, size_y, mapsize_x, mapsize_y);

            Step = null;
            Drawing = null;
            paused = true;
            ai.SpawnEntity += SpawnEntity;

            map = new int[14, 11]
                {
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                    { 0,0,1,1,1,1,1,1,1,0,0 },
                };
            spawn = new int[14, 11]
            {
                    { 1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 0,0,0,0,0,0,0,0,0,0,0 },
                    { 2,2,2,2,2,2,2,2,2,2,2 },
                    { 2,2,2,2,2,2,2,2,2,2,2 },
                    { 2,2,2,2,2,2,2,2,2,2,2 },
            };


            button1.Enabled = true;
            for (int i = 1; i < buttons.Count; i++)
            {
                buttons[i].Enabled = false;
            }
            button5.Enabled = true;

            
            game = new GameArgs(a, new List<Entity>(), new Cell[mapsize_y, mapsize_x], ai, paused1);
            for (int i = 0; i < mapsize_y * mapsize_x; i++)
            {
                game.cell[i / mapsize_x, i % mapsize_x] = new Cell(map[i / mapsize_x, i % mapsize_x], spawn[i / mapsize_x, i % mapsize_x]);
            }
            if (game.objects.Count != 0)
                for (int i = 0; i < game.objects.Count; i++)
                {
                    game.objects[i].DeleteMe -= DeleteEntity;
                }
            game.objects = new List<Entity>();
            Entity.ChangeObjects(game.objects);
            ai.PlaceBase(game.cell, mapsize_x);

            label1.Text = "Золото: " + Math.Round(a.gold) + "\n" + "Металл: " + Math.Round(a.metal) + "\n" + "Топливо: " + Math.Round(a.oil);
            //AI_place_base();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 10), 0, 0, Round(size_x, resizeX * coefWidth) * mapsize_x, Round(size_y, resizeY * coefHeight) * mapsize_y + aboutToolStripMenuItem.Size.Height); //рамка
            for (int i = 0; i < mapsize_y; i++) //поле
            {
                for (int j = 0; j < mapsize_x; j++)
                {
                    if (game.cell[i, j].ground_type == 0) e.Graphics.FillRectangle(new SolidBrush(Color.Blue), j * Round(size_x, resizeX * coefWidth), i * Round(size_y, resizeY * coefHeight) + aboutToolStripMenuItem.Size.Height, Round(size_x, resizeX * coefWidth), Round(size_y, resizeY * coefHeight));
                    else e.Graphics.FillRectangle(new SolidBrush(Color.LightGreen), j * Round(size_x, resizeX * coefWidth), i * Round(size_y, resizeY * coefHeight) + aboutToolStripMenuItem.Size.Height, Round(size_x, resizeX * coefWidth), Round(size_y, resizeY * coefHeight));
                }
            }
            if (button_pressed) //курсор для размещения
            {
                cursor_coords = this.PointToClient(Cursor.Position);
                point = GetPoint(cursor_coords);

                if (point.X != -1)
                    if (game.cell[point.Y,point.X].spawn_ability == 2)
                    e.Graphics.FillEllipse(new SolidBrush(Color.Red), point.X * Round(size_x, resizeX * coefWidth) + Round(size_x, resizeX * coefWidth) / 4, point.Y * Round(size_y, resizeY * coefHeight) + Round(size_y, resizeY * coefHeight) / 4 + aboutToolStripMenuItem.Size.Height,  Round(size_x, resizeX * coefWidth) / 2, Round(size_y, resizeY * coefHeight) / 2);

            }

            Drawing?.Invoke(Round(size_x, resizeX * coefWidth), Round(size_y, resizeY * coefHeight) + aboutToolStripMenuItem.Size.Height, resizeY * coefHeight, e);

        }
        public static int Round(int value, double coef)
        {
            return (int)(Math.Round(((double)value * coef)));
        }

        private Point GetPoint(Point coords)
        {
            if (coords.X < Round(size_x, resizeX * coefWidth) * (mapsize_x) & coords.X >= 0 & coords.Y >= 0 & coords.Y < Round(size_y, resizeY * coefHeight) * (mapsize_y))
                return new Point(coords.X / Round(size_x, resizeX * coefWidth), coords.Y / Round(size_y, resizeY * coefHeight));
            else return new Point(-1, -1);
        }

       
    }
}
