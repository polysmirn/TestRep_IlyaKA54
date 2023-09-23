using System;
using System.Collections.Generic;

namespace Call_to_Kursovaya
{
    [Serializable]
    public struct GameArgs
    {
        public ActionEventArgs args;
        public List<Entity> objects;

        public AI ai;
        public Cell[,] cell;
        public bool paused;
        public GameArgs(ActionEventArgs args, List<Entity> objects, Cell[,] cell, AI ai, bool paused)
        {
            this.args = args;
            this.objects = objects;
            this.cell = cell;
            this.paused = paused;
            this.ai = ai;
        }
    }
}
