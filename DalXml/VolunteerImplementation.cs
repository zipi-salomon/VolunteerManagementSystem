namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class VolunteerImplementation : IVolunteer
{/// <summary>
 /// help method
 /// </summary>
 /// <param name="v">volunteer</param>
 /// <returns>new volunteer</returns>
 /// <exception cref="FormatException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            Name = (string?)v.Element("Name") ?? "",
            Phone = (string?)v.Element("Phone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            MaxDistanceForCall = (double?)v.Element("MaxDistanceForCall") ?? null,
            Address = (string?)v.Element("Address") ?? null,
            Password = (string?)v.Element("Password") ?? "",
            Longitude = (double?)v.Element("Longitude") ?? null,
            Latitude = (double?)v.Element("Latitude") ?? null,
            DistanceType = v.ToEnumNullable<DistanceType>("DistanceType") ?? DistanceType.walking,
            Active = (bool?)v.Element("Active") ?? false,
            Role = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer

        };
    }

    /// <summary>
    /// method to create volunteer
    /// </summary>
    /// <param name="item">volunteer to create</param>
    /// <exception cref="Exception"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Volunteer item)
    {

        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        if (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        XElement newVol = new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("Name", item.Name),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("Role", item.Role),
            new XElement("Active", item.Active),
            new XElement("DistanceType", item.DistanceType),
            item.Latitude == null ? null : new XElement("Latitude", item.Latitude),
             item.Longitude == null ? null : new XElement("Longitude", item.Longitude),
            new XElement("Password", item.Password),
            new XElement("Address", item.Address),
            new XElement("MaxDistanceForCall", item.MaxDistanceForCall)
            );
        volunteersRootElem.Add(newVol);
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// method to delete volunteer
    /// </summary>
    /// <param name="id">volunteer's id to delete</param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {

        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does Not exist"))
                .Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);

    }

    /// <summary>
    /// method to delete all the volunteers
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    }

    /// <summary>
    /// method to read volunteer
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>the volunteer</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    /// <summary>
    /// method to read volunteer by id
    /// </summary>
    /// <param name="id">volunteer's id to read</param>
    /// <returns>volunteer</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem =
        XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    /// <summary>
    /// method to read all the volunteers
    /// </summary>
    /// <param name="filter">boolian function to filter the data to be returned</param>
    /// <returns>all the volunteers</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return filter == null ?
            Volunteers.Select(item => item) :
            Volunteers.Where(filter);
    }

    /// <summary>
    /// method to update volunteer
    /// </summary>
    /// <param name="item">item to update</param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();
        XElement newVol = new XElement("Volunteer",
    new XElement("Id", item.Id),
    new XElement("Name", item.Name),
    new XElement("Phone", item.Phone),
    new XElement("Email", item.Email),
    new XElement("Role", item.Role),
    new XElement("Active", item.Active),
    new XElement("DistanceType", item.DistanceType),
    item.Latitude == null ? null : new XElement("Latitude", item.Latitude),
    item.Longitude == null ? null : new XElement("Longitude", item.Longitude),
    new XElement("Password", item.Password),
    item.Address == null ? null : new XElement("Address", item.Address),
    item.MaxDistanceForCall == null ? null : new XElement("MaxDistanceForCall", item.MaxDistanceForCall)
);

        volunteersRootElem.Add(newVol);
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
}
