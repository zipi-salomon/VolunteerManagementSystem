namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{/// <summary>
/// מתודה להוספת קריאה חדשה 
/// </summary>
/// <param name="item">אובייקט קיראה להוספת</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int callId = Config.NextCallId;
        Call newCall=item with { Id = callId };
        DataSource.Calls.Add(newCall);
    }
    /// <summary>
    /// מתודה למחיקת קיראה ע"פ מספר שלה
    /// </summary>
    /// <param name="id">מספר קריאה</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exists");
        DataSource.Calls.Remove(Read(id)!);
    }
    // מתודה למחיקת כל הקריאות

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();   
    }

    /// <summary>
    ///מתודה לקריאת הקריאה ע"פ מספר זיהוי 
    /// </summary>
    /// <param name="id">מספר זיהוי של קריאה</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(call => call.Id == id);
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>one data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
     => DataSource.Calls.FirstOrDefault(filter);

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>all or part of the data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
       => filter == null
          ? DataSource.Calls.Select(item => item)
           : DataSource.Calls.Where(filter);
    /// <summary>
    /// מתודה לעדכון קריאה ע"פ המספר שלה
    /// </summary>
    /// <param name="item">אובייקט קריאה לעדכון</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        if (Read(item.Id) is null)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exists");
        DataSource.Calls.Remove(Read(item.Id)!);
        DataSource.Calls.Add(item);
    }
}
