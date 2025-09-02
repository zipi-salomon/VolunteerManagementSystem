using BlApi;
using Helpers;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Advance the system clock by the appropriate time unit.
    /// </summary>
    /// <param name="timeUnit">The unit of time for promotion</param>
    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        switch (timeUnit)
        {
            case BO.TimeUnit.Minute: AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1)); break;
            case BO.TimeUnit.Hour: AdminManager.UpdateClock(AdminManager.Now.AddHours(1)); break;
            case BO.TimeUnit.Day: AdminManager.UpdateClock(AdminManager.Now.AddDays(1)); break;
            case BO.TimeUnit.Month: AdminManager.UpdateClock(AdminManager.Now.AddMonths(1)); break;
            case BO.TimeUnit.Year: AdminManager.UpdateClock(AdminManager.Now.AddYears(1)); break;
        };

    }
    /// <summary>
    /// Returns the value of the system clock.
    /// </summary>
    /// <returns> the value of the system clock. </returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }
    /// <summary>
    /// Return the value of the configuration variable "Risk Range"
    /// </summary>
    /// <returns>The value of the configuration variable "Risk Range"</returns>
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }
    /// <summary>
    /// Initialize the database.
    /// </summary>
    public void InitializeDB() //stage 4
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    /// <summary>
    /// Reset all configuration data (reset all configuration data to its initial value)
    /// Clear all entity data(clear all data lists)
    /// </summary>
    public void ResetDB() //stage 4
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    /// <summary>
    /// Updates the value of the configuration variable "Risk Time Range" to the value received as a parameter
    /// </summary>
    /// <param name="newRiskRange">Risk time frame</param>
    public void SetRiskRange(TimeSpan newRiskRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.RiskRange = newRiskRange;
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

    public void StartSimulator(int interval)  //stage 7
    {
        //AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

}