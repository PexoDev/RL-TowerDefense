namespace Assets.Scripts.Units
{
    public static class DamageHelper
    {
        public const float DamageIncrease = 2f;
        public const float DamageDecrease = 0f;
        public static int CalculateElementalDamage(int damage, Element attackerElement, Element targetElement)
        {
            if (attackerElement == Element.Fire && targetElement == Element.Ice)
                return (int)(damage * DamageDecrease);

            if (attackerElement == Element.Fire && targetElement == Element.Forest)
                return (int)(damage * DamageIncrease);

            if (attackerElement == Element.Ice && targetElement == Element.Forest)
                return (int)(damage * DamageDecrease);

            if (attackerElement == Element.Ice && targetElement == Element.Fire)
                return (int)(damage * DamageIncrease);

            if (attackerElement == Element.Forest && targetElement == Element.Fire)
                return (int)(damage * DamageDecrease);

            if (attackerElement == Element.Forest && targetElement == Element.Ice)
                return (int)(damage * DamageIncrease);

            return damage;
        }
    }
}