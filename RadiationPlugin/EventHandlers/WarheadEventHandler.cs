using PluginAPI.Events;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using PluginAPI.Core;
using MEC;
using RadiationPlugin.StatusEffects;

namespace RadiationPlugin.EventHandlers
{
    public class WarheadEventHandler
    {
        //Controls the intensity of the effect.
        public static byte CurrentMode = 0;
        //Controls how long before the player begins gaining the raidation effect
        public static Dictionary<ReferenceHub, float> Exposure = new Dictionary<ReferenceHub, float>();
        //Used to find if the player is still within the HCZ_Warhead room
        public static DoorVariant NukeDoor = null;

        //Controls checking for where the player is
        public float HigherBound = 0f;
        public float LowerBound = 0f;

        [PluginEvent(ServerEventType.WarheadStop)]
        public void OnWarheadCancelled(WarheadStopEvent ev)
        {
            if(AlphaWarheadController.TimeUntilDetonation > 80)
            {
                CurrentMode = 0;
            }
            if(AlphaWarheadController.TimeUntilDetonation < 80 && AlphaWarheadController.TimeUntilDetonation > 50)
            {
                CurrentMode = 1;
            }
            if (AlphaWarheadController.TimeUntilDetonation < 50 && AlphaWarheadController.TimeUntilDetonation > 30)
            {
                CurrentMode = 2;
            }
            if(AlphaWarheadController.TimeUntilDetonation < 30)
            {
                CurrentMode = 3;
            }
        }

        [PluginEvent(ServerEventType.RoundStart)]
        public void OnRoundStart(RoundStartEvent ev)
        {
            //Reset vars
            CurrentMode = 0;
            Exposure.Clear();
            //Find the door
            List<RoomIdentifier> searchResults = RoomIdUtils.FindRooms(RoomName.HczWarhead, FacilityZone.HeavyContainment, RoomShape.Undefined).ToList();
            RoomIdentifier warheadRoom = searchResults.First();
            HashSet<DoorVariant> warheadDoors = DoorVariant.DoorsByRoom[warheadRoom];
            DoorVariant armoryDoor = warheadDoors.Where(x => x.TryGetComponent<DoorNametagExtension>(out DoorNametagExtension name) && name.GetName == "NUKE_ARMORY").FirstOrDefault();
            if(armoryDoor == null)
            {
                Log.Error("Can't find NUKE ARMORY!");
            }
            NukeDoor = armoryDoor;
            //Basically making sure the player is within these bounds
            HigherBound = armoryDoor.transform.localPosition.y + 10f;
            LowerBound = armoryDoor.transform.localPosition.y - 10f;
            //Log.Debug("Starting Loop!");
            Timing.CallPeriodically(float.MaxValue, Plugin.GetConfig().CheckInterval, () =>
            {
                CheckPlayers();
            });
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        public void OnUseItem(PlayerUsedItemEvent ev)
        {
            if(ev.Item.ItemTypeId == ItemType.SCP500)
            {
                Exposure[ev.Player.ReferenceHub] = 0f;
            }
        }




        [PluginEvent(ServerEventType.PlayerSpawn)]
        public void OnPlayerRoleChange(PlayerSpawnEvent ev)
        {
            Exposure[ev.Player.ReferenceHub] = 0f;
        }

        private void CheckPlayers()
        {
            //Log.Debug("Updating Players!");
            HashSet<ReferenceHub> affectedPlayers = new HashSet<ReferenceHub>();
            foreach(ReferenceHub hub in ReferenceHub.AllHubs)
            {
                RoomIdentifier room = RoomIdUtils.FindRooms(RoomName.HczWarhead, FacilityZone.HeavyContainment, RoomShape.Undefined).First();
                if(RoomIdUtils.IsWithinRoomBoundaries(room, hub.gameObject.transform.position))
                {
                    //Log.Debug("Player in nuke room: " + hub.nicknameSync.DisplayName);
                }
                else
                {
                    continue;
                }
                Player target = Player.Get(hub);
                if (hub.transform.position.y > -999f && target.Zone == FacilityZone.HeavyContainment)
                {
                    //Log.Debug("Player near door: " + hub.nicknameSync.DisplayName);
                    affectedPlayers.Add(hub);
                    if (!Exposure.ContainsKey(hub))
                    {
                        Log.Debug("Player not in dict, adding");
                        Exposure.Add(hub, 0f);
                    }
                }
                else
                {
                    continue;
                }
            }
            foreach (ReferenceHub player in affectedPlayers)
            {
                float targetExposure = Exposure[player];
                if(targetExposure >= Plugin.GetConfig().MaxExposure)
                {
                    Log.Debug("Giving radiation!");
                    float effectDuration = Plugin.GetConfig().CheckInterval + Plugin.GetConfig().EffectDurationBuffer;
                    byte intensity = Plugin.GetConfig().IntensityPerTime[CurrentMode];
                    player.playerEffectsController.ChangeState<Radiation>(intensity, effectDuration, false);
                }
                if(CurrentMode != 0)
                {
                    Log.Debug("Giving exposure!");
                    Exposure[player] += Plugin.GetConfig().CheckInterval;
                }
            }
        }
    }
}
