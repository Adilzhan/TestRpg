namespace Conf
{
    public class MeleeMonkUnitInfo : UnitInfo
    {
        public int AbilityShieldHealth;
        public int AbilityShieldAbsorbingPercent;
        public int AbilityShieldExplosionDamage;
        public int AbilityShieldExplosionHeal;
        public int HealDamagePercent;

        public MeleeMonkUnitInfo()
        {
            MaxHealth = 1800;
            MaxMana = 100;
            Speed = 3;
            Damage = 5;
            ManaRegen = 5;
            AttackDistance = 2;
            AbilityShieldHealth = 100;//жизнь щита 
            AbilityShieldAbsorbingPercent = 50;//блокируемый урон
            AbilityShieldExplosionDamage = 250;//взрыв наносящий урон
            AbilityShieldExplosionHeal = 100;//взрыв лечащий 
            HealDamagePercent = 10;//процент лечения
            StunOutDamage = 100;
        }
    }
}
