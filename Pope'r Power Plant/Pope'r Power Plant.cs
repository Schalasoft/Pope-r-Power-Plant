using Harmony;
using STRINGS;
using System;
using static RoomConstraints;

namespace PoperPowerPlant
{
    // Attach to RoomTypes constructor
    [HarmonyPatch(typeof(Database.RoomTypes), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(ResourceSet) })]
    internal class PoperGenerator_RoomTypes
    {
        public static void Postfix(ref Database.RoomTypes __instance)
        {
            //Debug.Log(" === PoperPowerPlant_RoomTypes Postfix === ");

            // Get Power Plant
            RoomType roomType = __instance.TryGet(__instance.PowerPlant.Id);

            // Custom max size constraint for room
            int customMaxSize = 240;
            Constraint MaxSizeConstraint = new Constraint(null, (Room room) => room.cavity.numCells <= customMaxSize,
                1,
                string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "" + customMaxSize),
                string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "" + customMaxSize),
                null);

            // Custom max size for all rooms
            TuningData<RoomProber.Tuning>.Get().maxRoomSize = customMaxSize;

            // Iterate constraints
            RoomConstraints.Constraint[] additional_constraints = roomType.additional_constraints;
            for (int i = 0; i < additional_constraints.Length; i++)
            {
                // Change max size constraint
                if (additional_constraints[i].name.Contains("Maximum size:"))
                {
                    additional_constraints[i] = MaxSizeConstraint;
                    break;
                }
            }
        }
    }
}
