namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.Exceptions;

public class LocalAuthorityNotFoundException : Exception
{
    public LocalAuthorityNotFoundException(string name) : base($"Local Authority with name {name} was not found.")
    {
    }

    public LocalAuthorityNotFoundException(string[] messages) : base(string.Join("=>",messages)) { }
    public LocalAuthorityNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}