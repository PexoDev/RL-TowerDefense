using UnityEngine;

namespace Assets.Scripts.Units
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Wave")]
    public class Wave : ScriptableObject
    {
        public UnitData[] Units;
    }
}