using System;
using System.Drawing;

namespace Call_to_Kursovaya
{
    [Serializable]
    public class Building : Entity
    {
        protected double gold_income, metal_income, oil_income;

        public Building(int _ally, Point point) : base(_ally, point)
        {
            entity_class = Unit_type.BUILDING;
        }
        public override void Do(ActionEventArgs a)
        {
            if (this.health <= 0) DeleteIt();

            if (this.ally == 2)
            {
                a.gold += gold_income;
                a.metal += metal_income;
                a.oil += oil_income;
            }
            else
            {
                a.ai_gold += gold_income;
                a.ai_metal += metal_income;
                a.ai_oil += oil_income;
            }
        }

    }
}
