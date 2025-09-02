using System.Runtime.Serialization;

namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception ex) : base(message, ex) { }

}
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message, Exception ex) : base(message, ex) { }
}

[Serializable]
public class BlCantDeleteException : Exception
{
    public BlCantDeleteException(string? message, Exception ex) : base(message, ex) { }
    public BlCantDeleteException(string? message) : base(message) { }

}
[Serializable]
public class BlCantUpdateException : Exception
{
    public BlCantUpdateException(string? message, Exception ex) : base(message, ex) { }
    public BlCantUpdateException(string? message) : base(message) { }
}



[Serializable]
public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
    public BlInvalidValueException(string? message, Exception ex) : base(message, ex) { }
}
[Serializable]
public class BlUnauthorizedException:Exception
{
    public BlUnauthorizedException(string? message):base(message) { }
}
[Serializable]
public class BlInvalidRequestException : Exception
{
    public BlInvalidRequestException(string? message) : base(message) { }
    public BlInvalidRequestException(string? message, Exception ex) : base(message, ex) { }

}
[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
}

