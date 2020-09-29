using System;
using Conf;
using Core;


namespace Logic
{
    public class MeleeHedgehogUnitLogic : UnitLogic
    {
               
        private readonly int _attackDistance;
        private readonly int _healHpThreshold;
        private readonly int _healValue;
        private readonly int _abilityDamageIncreaseStep;
        private readonly int _damage;
        private readonly int _manaRegen;
        
        private int _abilityDamage;
        private bool _hasStun;
        private int step;
    
        public MeleeHedgehogUnitLogic(MeleeHedgehogUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _abilityDamage = 0;
            _healHpThreshold = (int)Math.Round(info.MaxHealth * info.HealHpThresholdPercent / 100f);
            _healValue = info.HealValue;
            _attackDistance = info.AttackDistance;
            _abilityDamageIncreaseStep = info.AbilityDamageIncreaseStep;
        }
    
        public override void OnTurn()
        {
            if (!_hasStun)
             {
            step=0;
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
            else
            {
                step+=1; 
                if (step>=3)
                {
                    _hasStun=true;
                    step=0;
                } 
            }
        }

    
        public override void OnAbility()
        {
            var target = Core.GetNearestEnemy(Unit);
            if (target != null && target.IsAlive() && Core.GetDistance(Unit, target) < _attackDistance)
            {
                target.Damage(_abilityDamage);
            }
        }
    
        public override int OnDamage(int damage)
        {
            if (damage >= _healHpThreshold)
            {
                Unit.Heal(_healValue);
            }
            if(damage>=100)
            {
                _hasStun=true;
                step=0;
            }
            return damage;
        }
    
        public override int OnBeforeManaChange(int delta)
        {
            if (delta > 0)
            {
                _abilityDamage += _abilityDamageIncreaseStep;
                delta = 0;
            }
            return delta;
        }

    }
}