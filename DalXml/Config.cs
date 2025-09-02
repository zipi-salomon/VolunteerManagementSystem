using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    static internal class Config
    {
        internal const string s_data_config_xml = "data-config.xml";
        internal const string s_volunteers_xml = "volunteers.xml";
        internal const string s_calls_xml = "calls.xml";
        internal const string s_assignments_xml = "assignments.xml";

        internal static int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
        }
        internal static int NextAssignmentId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
        }
        internal static DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }
        internal static TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }
        /// <summary>
        /// reset all the config variables.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void Reset()
        {
            Clock = DateTime.Now;
            NextCallId = 1;
            NextAssignmentId = 1;
            RiskRange = TimeSpan.Zero;
        }
    }

}
