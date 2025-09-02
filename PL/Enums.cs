using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class CallTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallType> s_enums =
    (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class RoleCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Role> s_enums =
    (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class DistanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class StatusCallCollection : IEnumerable
    {
        static readonly IEnumerable<BO.StatusCall> s_enums =
    (Enum.GetValues(typeof(BO.StatusCall)) as IEnumerable<BO.StatusCall>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }


    internal class VolunteerInListAttributesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListAttributes> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerInListAttributes)) as IEnumerable<BO.VolunteerInListAttributes>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class OpenCallInListAttributesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.OpenCallInListAttributes> s_enums =
        (Enum.GetValues(typeof(BO.OpenCallInListAttributes)) as IEnumerable<BO.OpenCallInListAttributes>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class ClosedCallInListAttributesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.ClosedCallInListAttributes> s_enums =
        (Enum.GetValues(typeof(BO.ClosedCallInListAttributes)) as IEnumerable<BO.ClosedCallInListAttributes>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class CallInListAttributesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallInListAttributes> s_enums =
        (Enum.GetValues(typeof(BO.CallInListAttributes)) as IEnumerable<BO.CallInListAttributes>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}
