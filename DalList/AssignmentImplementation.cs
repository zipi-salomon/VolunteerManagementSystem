namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// מתודה ליצירת הקצאה חדשה 
    /// </summary>
    /// <param name="item">הקצאה חדשה להוספה</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        int assignmentId = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = assignmentId };
        DataSource.Assignments.Add(newAssignment);
    }
    /// <summary>
    /// מתודה למחיקת הקצאה ע"פ מספרה
    /// </summary>
    /// <param name="id">מספר הקצאה</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exists");
        DataSource.Assignments.Remove(Read(id)!);
    }
    //מתודה למחיקת כל ההקצאות
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    ///מתודה לקריאת הקצאה ע"פ מספר זיהוי 
    /// </summary>
    /// <param name="id">מספר זיהוי של הקצאה</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(assignment => assignment.Id == id);
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>one data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
     => DataSource.Assignments.FirstOrDefault(filter);

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>all or part of the data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
      => filter == null
          ? DataSource.Assignments.Select(item => item)
          : DataSource.Assignments.Where(filter);

    /// <summary>
    ///  מתודה לעדכון הקצאה ע"פ המספר שלה
    /// </summary>
    /// <param name="item">אובייקט הקצאה לעדכון</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        if (Read(item.Id) is null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exists");
        DataSource.Assignments.Remove(Read(item.Id)!);
        DataSource.Assignments.Add(item);
    }
}
