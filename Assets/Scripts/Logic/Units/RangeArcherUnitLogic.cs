using Conf;
using Core;

namespace Logic
{
    public class RangeArcherUnitLogic : UnitLogic
    {
        private readonly int _attackDistance;
        private readonly int _killChance;
        private readonly int _damage;
        private readonly int _manaRegen;
        private readonly int _stunOutDamage;
        private int _ReceivedDamage;
        private int _stunned;

        public RangeArcherUnitLogic(RangeArcherUnitInfo info, IUnit unit, ICore core) : base(unit, core)
        {
            _damage = info.Damage;
            _manaRegen = info.ManaRegen;
            _attackDistance = info.AttackDistance;
            _killChance = info.KillChance;
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
            if (target != null && target.IsAlive() && UnityEngine.Random.Range(0, 100) < _killChance)
            {
                target.Damage(target.MaxHealth);
            }
            else
            {   if (target!= null && target.IsAlive()){
                target.Stun();// 25% шанс криты не вылетел и оглушил цель
                UnityEngine.Debug.Log("STUN!");}
            }
        }
	
         public override int OnDamage(int damage)//полученный урон
        {
            Unit.AddMana(_manaRegen);
            _ReceivedDamage += damage;//накапливание урона полученного за шаг
            _stunned = _ReceivedDamage > _stunOutDamage ? 0 : _stunned;//если полученный урон <100 обьект в Оглушен если больше 100 то Стан снимается 
            return damage;
        }

        public override void OnStun()
        {
            _stunned = 3;//шаги пока обьект будет оглушен если только не выполнится условие выше
        }
    }
}