using UnityEngine;

namespace Anoa.Module
{
    public class CooldownModule
    {
        public bool IsReady => Time.time >= fltLastUsedTime + fltCooldown;

        private float fltCooldown;
        private float fltLastUsedTime;

        public CooldownModule(float _fltCooldown, bool _useNow = false)
        {
            fltCooldown = _fltCooldown;
            fltLastUsedTime = Time.time;

            if (_useNow)
                fltLastUsedTime -= _fltCooldown;
        }

        public void Refresh()
        {
            fltLastUsedTime -= fltCooldown;
        }

        public void Update(float _fltCooldown)
        {
            fltCooldown = _fltCooldown;
        }

        public float GetRemainingTime()
        {
            return fltLastUsedTime + fltCooldown - Time.time;
        }

        public void Use()
        {
            fltLastUsedTime = Time.time;
        }
    }
}