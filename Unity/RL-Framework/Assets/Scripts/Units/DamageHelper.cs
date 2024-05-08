namespace Assets.Scripts.Units
{
    public static class DamageHelper
    {
        public static int CalculateElementalDamage(int damage, Element attackerElement, Element targetElement)
        {
            if (attackerElement == Element.Fire && targetElement == Element.Ice)
                return damage / 2;

            if (attackerElement == Element.Fire && targetElement == Element.Forest)  
                return damage * 2;

            if (attackerElement == Element.Ice && targetElement == Element.Forest)
                return damage / 2;

            if (attackerElement == Element.Ice && targetElement == Element.Fire)
                return damage * 2;

            if (attackerElement == Element.Forest && targetElement == Element.Fire)
                return damage / 2;

            if (attackerElement == Element.Forest && targetElement == Element.Ice)
                return damage * 2;

            return damage;
        }
    }
}