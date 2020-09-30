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
        private readonly int _stunOutDamage;
        private int _ReceivedDamage;
        private int _stunned;
       
    
        public MeleeHedgehogUnitLogic(MeleeHedgehogUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _abilityDamage = 0;
            _healHpThreshold = (int)Math.Round(info.MaxHealth * info.HealHpThresholdPercent / 100f);
            _healValue = info.HealValue;
            _attackDistance = info.AttackDistance;
            _abilityDamageIncreaseStep = info.AbilityDamageIncreaseStep;
            _stunOutDamage = info.StunOutDamage; //я бы мог бы просто указать тут 100 но 
                                                //хотел чтобы для каждого юнита был свой порог выхода из оглушения
            _ReceivedDamage = 0;
            _stunned = 0;
        }
    
        public override void OnTurn()
        {
            if (_stunned == 0)//если обьект не оглушен
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
            else
            {
                _stunned = System.Math.Max(0, _stunned - 1);
				_ReceivedDamage = 0;
            }
               
        }
    
        public override void OnAbility()
        {
            var target = Core.GetNearestEnemy(Unit);
            if (target != null && target.IsAlive() && Core.GetDistance(Unit, target) < _attackDistance)
            {
                target.Damage(_abilityDamage);
                _abilityDamage = 0;//сброс накопленного урона копим сначала

            }
        }
    
        public override int OnDamage(int damage)
        {
            _ReceivedDamage += damage;//накапливание урона полученного за шаг
            _stunned = _ReceivedDamage > _stunOutDamage ? 0 : _stunned;//если полученный урон <100 обьект в Оглушен если больше 100 то Стан снимается 
            if (_stunned == 0)//если обьект не оглушен поаналогии с маной 
            {
                if (damage >= _healHpThreshold)
                {
                    Unit.Heal(_healValue);
                }
            }
                    return damage;
        }
    
        public override int OnBeforeManaChange(int delta)
        {
            
            if (delta > 0)
            {
                _abilityDamage += _abilityDamageIncreaseStep; //удалил delta = 0 так как обьект не использовал криту
            }
            return delta;
        }

        public override void OnStun()
        {
            _stunned = 3;//шаги пока обьект будет оглушен если только не выполнится условие выше
        }
    }

}