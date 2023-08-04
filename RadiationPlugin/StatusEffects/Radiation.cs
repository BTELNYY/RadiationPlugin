using CustomPlayerEffects;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiationPlugin.StatusEffects
{
    public class Radiation : TickingEffectBase
    {
        protected override void Disabled()
        {
            base.Disabled();
            _currentTicks = 0;
        }

        protected override void Enabled()
        {
            base.Enabled();
            _currentTicks = 0;
            CurrentDamage = MinDamage * Intensity;
        }

        protected override void OnTick()
        {
            int multiplyAmount = (int)Math.Floor((float)(_currentTicks / MultiplyDamageEveryTicks));
            CurrentDamage = MinDamage * Intensity;
            for (int i = 0; i < multiplyAmount; i++)
            {
                CurrentDamage *= DamageMultiplier;
            }
            CustomReasonDamageHandler handler = new CustomReasonDamageHandler("Lack of hair, Pale skin and Skin irritation suggest the cause as Acute Radiation Sickness", CurrentDamage);
            Hub.playerEffectsController.ServerSendPulse<Poisoned>();
            Hub.playerStats.DealDamage(handler);
            _currentTicks++;
        }

        private int _currentTicks = 0;
        public float MinDamage = 1f;
        public float CurrentDamage = 1f;
        public int MultiplyDamageEveryTicks = 15;
        public float DamageMultiplier = 2f;
    }
}
