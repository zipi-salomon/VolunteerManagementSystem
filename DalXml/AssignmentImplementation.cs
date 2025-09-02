namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{    /// <summary>
     /// method to create new assignment 
     /// </summary>
     /// <param name="item">new assignment to create</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        int assignmentId = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = assignmentId };
        Assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// method to delete assignment by id
    /// </summary>
    /// <param name="id">assignment id</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// method to delete all assignments
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml );
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>one data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(Func<Assignment, bool> filter)
      => XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml).FirstOrDefault(filter);

    /// <summary>
    ///method  to read assignment by the id
    /// </summary>
    /// <param name="id">assignment id</param>
    /// <returns>the assignment</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment? Read(int id)
    {
        return XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml).FirstOrDefault(assignment => assignment.Id == id);
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>all or part of the data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
              return  filter == null
          ? Assignments.Select(item => item)
          : Assignments.Where(filter);
    }

    /// <summary>
    ///method to update assignment 
    /// </summary>
    /// <param name="item">call object to assignment</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
}
