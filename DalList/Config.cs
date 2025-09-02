using System.Runtime.CompilerServices;

namespace Dal
{
    internal static class Config
    {
        internal const int startCallId = 1;
        private static int nextCallId = startCallId;
        internal static int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => nextCallId++; }

        internal const int startAssignmentId = 1;
        private static int nextAssignmentId = startAssignmentId;
        internal static int NextAssignmentId {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => nextAssignmentId++; }

        internal static DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set; } = DateTime.Now;
        internal static TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set; } = TimeSpan.MinValue;
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void Reset()
        {
            Clock= DateTime.Now;
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            RiskRange = TimeSpan.Zero;
        }
    }
}
