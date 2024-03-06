using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace InfiniteReserveAmmo
{
    public class InfiniteReserveAmmo : BasePlugin
    {
        public override string ModuleName => "Infinite Reserve Ammo";
        public override string ModuleVersion => "1.0";
        public override string ModuleAuthor => "Oylsister";

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventWeaponFire>(OnWeaponFire, HookMode.Post);
        }

        private HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo info)
        {
            var client = @event.Userid;
            if (client != null && client.IsValid)
            {
                SetInfiniteBullet(client);
            }

            return HookResult.Continue;
        }

        void SetInfiniteBullet(CCSPlayerController client)
        {
            if (client.PawnIsAlive && client.LifeState == (byte)LifeState_t.LIFE_ALIVE)
            {
                var weapon = client.PlayerPawn.Value!.WeaponServices!.ActiveWeapon;
                var weaponbase = weapon.Value!.As<CCSWeaponBase>();
                if (weapon != null && (weaponbase.VData!.GearSlot == gear_slot_t.GEAR_SLOT_RIFLE || weaponbase.VData!.GearSlot == gear_slot_t.GEAR_SLOT_PISTOL))
                {
                    SetReserveAmmo(client, GetReserveAmmo(client) + 1);
                }
            }
        }

        int GetReserveAmmo(CCSPlayerController client)
        {
            var weapon = client.PlayerPawn.Value!.WeaponServices!.ActiveWeapon;

            if (weapon == null || !weapon.IsValid)
                return -1;

            var ammo = weapon.Value!.ReserveAmmo[0];

            return ammo;
        }

        void SetReserveAmmo(CCSPlayerController client, int ammo)
        {
            var weapon = client.PlayerPawn.Value!.WeaponServices!.ActiveWeapon;

            if (weapon == null || !weapon.IsValid)
                return;

            weapon.Value!.ReserveAmmo[0] = ammo;

            Utilities.SetStateChanged(weapon.Value.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
        }
    }
}
