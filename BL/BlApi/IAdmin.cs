namespace BlApi
{
    public interface IAdmin
    {
        /// <summary>
        /// Returns the value of the system clock.
        /// </summary>
        /// <returns> the value of the system clock. </returns>
        DateTime GetClock();

        /// <summary>
        /// Advance the system clock by the appropriate time unit.
        /// </summary>
        /// <param name="timeUnit">The unit of time for promotion</param>
        void AdvanceClock(BO.TimeUnit timeUnit);

        /// <summary>
        /// Return the value of the configuration variable "Risk Range"
        /// </summary>
        /// <returns>The value of the configuration variable "Risk Range"</returns>
        TimeSpan GetRiskRange();

        /// <summary>
        /// Updates the value of the configuration variable "Risk Time Range" to the value received as a parameter
        /// </summary>
        /// <param name="newRiskRange">Risk time frame</param>
        void SetRiskRange(TimeSpan newRiskRange);

        /// <summary>
        /// Reset all configuration data (reset all configuration data to its initial value)
        /// Clear all entity data(clear all data lists)
        /// </summary>
        void ResetDB();

        /// <summary>
        /// Initialize the database.
        /// </summary>
        void InitializeDB();
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5

        void StartSimulator(int interval); //stage 7
        void StopSimulator(); //stage 7
    }
}