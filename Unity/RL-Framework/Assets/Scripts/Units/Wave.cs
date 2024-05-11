using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
    public class Wave : ScriptableObject
    {
        public UnitData FireGoblin;
        public UnitData FireOrc;
        public UnitData FireTroll;


        public UnitData IceGoblin;
        public UnitData IceOrc;
        public UnitData IceTroll;

        public UnitData ForestGoblin;
        public UnitData ForestOrc;
        public UnitData ForestTroll;

        private UnitData[] _units = null;
        public UnitData[] Units()
        {
            if(_units == null || _units.Length < 1)
                _units = InitializeUnits();
            return _units;
        }

        public UnitData[] InitializeUnits()
        {

            List<UnitData> units = new List<UnitData>();

            for (int i = 0; i < FireGoblins; i++)
                units.Add(Instantiate(FireGoblin));
            for(int i = 0; i < FireOrcs; i++)
                units.Add(Instantiate(FireOrc));
            for(int i = 0; i < FireTrolls; i++)
                units.Add(Instantiate(FireTroll));

            for(int i = 0; i < IceGoblins; i++)
                units.Add(Instantiate(IceGoblin));
            for(int i = 0; i < IceOrcs; i++)
                units.Add(Instantiate(IceOrc));
            for(int i = 0; i < IceTrolls; i++)
                units.Add(Instantiate(IceTroll));

            for(int i = 0; i < ForestGoblins; i++)
                units.Add(Instantiate(ForestGoblin));
            for(int i = 0; i < ForestOrcs; i++)
                units.Add(Instantiate(ForestOrc));
            for (int i = 0; i < ForestTrolls; i++)
                units.Add(Instantiate(ForestTroll));

            return units.ToArray();
        }

        public int FireGoblins;
        public int FireOrcs;
        public int FireTrolls;

        public int IceGoblins;
        public int IceOrcs;
        public int IceTrolls;

        public int ForestGoblins;
        public int ForestOrcs;
        public int ForestTrolls;
    }
}