using System;
using Conf;
using Core;

namespace Logic
{
    public class MeleeMonkUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _healDamagePercent;
        private readonly int _abilityShieldAbsorbingPercent;
        private readonly int _abilityShieldExplosionDamage;
        private readonly int _abilityShieldExplosionHeal;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunFreeDamage;

        private int _abilityShieldHealth;

        private int _ReceivedDamage;
        private int _stunned;

        public MeleeMonkUnitLogic(MeleeMonkUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _attackDistance = info.AttackDistance;
            _abilityShieldHealth = info.AbilityShieldHealth;
            _healDamagePercent = info.HealDamagePercent;
            _abilityShieldAbsorbingPercent = info.AbilityShieldAbsorbingPercent;
            _abilityShieldExplosionDamage = info.AbilityShieldExplosionDamage;
            _abilityShieldExplosionHeal = info.AbilityShieldExplosionHeal;
            _stunFreeDamage = info.StunOutDamage;//я бы мог бы просто указать тут 100 но 
                                                //хотел чтобы для каждого юнита был свой порог выхода из оглушения
            _ReceivedDamage = 0;
            _stunned = 0;
        }

        public override void OnTurn()
        {
            if (_stunned == 0)//если не в стане
            {
                var target = Core.GetNearestEnemy(Unit);
                if (target != null && target.IsAlive())
                {
                    if (Core.GetDistance(Unit, target) > _attackDistance)
                    {
                        Unit.MoveTo(target.X, target.Y);
                    }
                    else
                    {
                        target.Damage(_damage);
                    }
                }
                Unit.AddMana(_manaRegen);
            }
            Unit.Heal((int)Math.Round(_ReceivedDamage * _healDamagePercent / 100f));//лечит 10% от полученного урона
            _stunned = Math.Max(0, _stunned - 1);
            _ReceivedDamage = 0;
        }

        public override int OnDamage(int damage)
        {
            if (_abilityShieldHealth > 0)//если щит еще цел
            {
                _abilityShieldHealth -= (int)Math.Round(damage * _abilityShieldAbsorbingPercent / 100f);//поглощение 50% урона в пользу щита
                damage -= (int)Math.Round(damage * _abilityShieldAbsorbingPercent / 100f); //остальные 50% урона игроку
                damage += _abilityShieldHealth > 0 ? 0 : -_abilityShieldHealth;//проверка целостности щита
                if(_abilityShieldHealth < 0)//Если щит сломался, то он взрывается, нанося ближайшему противнику 250 урона и вылечивает владельца щита на 100 ХП.
                {
                    var target = Core.GetNearestEnemy(Unit);
                    if (target != null && target.IsAlive())
                    {
                        target.Damage(_abilityShieldExplosionDamage);
                    }
                    Unit.Heal(_abilityShieldExplosionHeal);
                }
            }
            _ReceivedDamage += damage;
            _stunned = _ReceivedDamage > _stunFreeDamage ? 0 : _stunned;
            return damage;
        }

        public override int OnBeforeManaChange(int delta)
        {
            if (delta > 0)
            {
                _abilityShieldHealth += _abilityShieldHealth > 0 ? _manaRegen : 0; //если щит цел то усиливаем маной
                delta = 0;
            }
            return delta;
        }

        public override void OnDie()
        {
            var target = Core.GetNearestFriend(Unit);// находим ближайшего союзника
            if (target != null && target.IsAlive())
            {
                target.Heal(_abilityShieldHealth);//лечим 
            }
        }

        public override void OnStun()
        {
            _stunned = 3;
        }
    }
}

