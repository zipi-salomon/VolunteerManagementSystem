﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    #endregion Stage 4

    #region Stage 5
    internal static event Action? ConfigUpdatedObservers; //prepared for stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers
    #endregion Stage 5

    #region Stage 4
    /// <summary>
    /// Property for providing/setting current configuration variable value for any BL class that may need it
    /// </summary>
    internal static TimeSpan RiskRange
    {
        get => s_dal.Config.RiskRange;
        set
        {
            s_dal.Config.RiskRange = value;
            ConfigUpdatedObservers?.Invoke(); // stage 5
        }
    }

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    internal static void ResetDB() //stage 4
    {
        lock (BlMutex) //stage 7
        {
            s_dal.ResetDB();
            AdminManager.UpdateClock(AdminManager.Now); //stage 5 - needed since we want the label on Pl to be updated
            AdminManager.RiskRange = AdminManager.RiskRange; // stage 5 - needed to update PL
        }
    }

    internal static void InitializeDB() //stage 4
    {
        lock (BlMutex) //stage 7
        {
            DalTest.Initialization.Do();
            AdminManager.UpdateClock(AdminManager.Now);  //stage 5 - needed since we want the label on Pl to be updated
            AdminManager.RiskRange = AdminManager.RiskRange; // stage 5 - needed for update the PL
        }
    }

    private static Task? _periodicTask = null;

    /// <summary>
    /// Method to perform application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        var oldClock = s_dal.Config.Clock; //stage 4
        s_dal.Config.Clock = newClock; //stage 4
        if (_periodicTask is null || _periodicTask.IsCompleted) //stage 7
            _periodicTask = Task.Run(() => CallManager.PeriodicCallsUpdates(oldClock, newClock));
        //etc ...

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }
    #endregion Stage 4

    #region Stage 7 base

    /// <summary>    
    /// Mutex to use from BL methods to get mutual exclusion while the simulator is running
    /// </summary>
    internal static readonly object BlMutex = new(); // BlMutex = s_dal; // This field is actually the same as s_dal - it is defined for readability of locks
    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static volatile Thread? s_thread;
    /// <summary>
    /// The Interval for clock updating
    /// in minutes by second (default value is 1, will be set on Start())    
    /// </summary>
    private static int s_interval = 1;
    /// <summary>
    /// The flag that signs whether simulator is running
    ///
    private static volatile bool s_stop = false;


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                
    public static void ThrowOnSimulatorIsRunning()
    {
        Debug.WriteLine(Thread.CurrentThread.ManagedThreadId );
        if ((_simulateTask?.Status==TaskStatus.Running) &&  (Thread.CurrentThread.ManagedThreadId==1 ))//זה כנראה התהליכון 
        //if (s_thread is not null)
            throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
        
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                
    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt(); //awake a sleeping thread
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }
    }

    private static Task? _simulateTask = null;

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));
            if (_simulateTask is null || _simulateTask.IsCompleted)//stage 7
                _simulateTask = Task.Run(() => {
                    lock (BlMutex)
                    {
                            VolunteerManager.VolunteerActivitySimulation();
                    }

                });

            ClockUpdatedObservers?.Invoke();
            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException)
            {
            }
        }
    }
    #endregion Stage 7 base
}