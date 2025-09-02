using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class AssignmentManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        public static DO.Call? GetCallByAssignment(DO.Assignment? assignment)
        {
            lock (AdminManager.BlMutex) //stage 7
            {
               return assignment == null ? null : s_dal.Call.Read(assignment.CallId);
            }
        }
    }
}
