namespace DO;
/// <summary>
///The exception will be thrown when an attempt is made to update/delete/request an 
///object with an ID number that does not exist in the list of objects of any type
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}
/// <summary>
/// The object already exist - the exception will be thrown , for example,
/// when trying to add to the list of objects of any type, an object with an
/// ID number that already exists
/// </summary>
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
/// <summary>
/// The exception will be thrown when no value is received from the user.
/// </summary>
[Serializable]
public class DalMustValueExeption : Exception
{
    public DalMustValueExeption(string? message) : base(message) { }
}
/// <summary>
/// Throw an exception when an xml file fails to create.
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}

