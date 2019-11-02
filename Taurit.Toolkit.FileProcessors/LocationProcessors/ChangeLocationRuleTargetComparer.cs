using System;
using System.Collections.Generic;

namespace Taurit.Toolkit.FileProcessors.LocationProcessors
{
    public class ChangeLocationRuleTargetComparer : IEqualityComparer<ChangeLocationRule>
    {
        /// <inheritdoc />
        public Boolean Equals(ChangeLocationRule x, ChangeLocationRule y)
        {
            if (x == null || y == null) return false;
            return x.TargetLocation == y.TargetLocation;
        }

        /// <inheritdoc />
        public Int32 GetHashCode(ChangeLocationRule obj)
        {
            return obj.TargetLocation.GetHashCode();
        }
    }
}