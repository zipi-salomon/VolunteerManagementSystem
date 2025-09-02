namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall

{/// <summary>
 ///method to create new call 
 /// </summary>
 /// <param name="item">call object to create</param>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int callId = Config.NextCallId;
        Call newCall = item with { Id = callId };
        Calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// method to delete call by id
    /// </summary>
    /// <param name="id">call id</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Calls with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// method to delete all calls
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>one data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Call? Read(Func<Call, bool> filter)
    => XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml).FirstOrDefault(filter);

    /// <summary>
    ///method  to read call by the id
    /// </summary>
    /// <param name="id">call id</param>
    /// <returns>the call</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Call? Read(int id)
    {
        return XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml).FirstOrDefault(call => call.Id == id);
    }

    /// <summary>
    /// method to return data.
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>all or part of the data</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ?
            Calls.Select(item => item) :
            Calls.Where(filter);
    }

    /// <summary>
    ///method to update call
    /// </summary>
    /// <param name="item">call object to call</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
