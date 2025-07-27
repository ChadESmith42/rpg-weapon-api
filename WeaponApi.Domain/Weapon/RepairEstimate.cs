using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeaponApi.Domain.Weapon
{
    public class RepairEstimate
    {
        public int RepairCost { get; }
        public int GainedHitPoints { get; }
        public decimal GainedValue { get; }

        public RepairEstimate(int repairCost, int gainedHitPoints, decimal gainedValue)
        {
            if (repairCost < 0)
                throw new ArgumentOutOfRangeException(nameof(repairCost), "Repair cost cannot be negative.");
            if (gainedHitPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(gainedHitPoints), "Gained hit points cannot be negative.");
            if (gainedValue < 0)
                throw new ArgumentOutOfRangeException(nameof(gainedValue), "Gained value cannot be negative.");

            RepairCost = repairCost;
            GainedHitPoints = gainedHitPoints;
            GainedValue = gainedValue;
        }
    }
}
