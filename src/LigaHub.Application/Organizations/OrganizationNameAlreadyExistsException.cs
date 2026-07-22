namespace LigaHub.Application.Organizations;

public sealed class OrganizationNameAlreadyExistsException : Exception
{
    public OrganizationNameAlreadyExistsException(string name)
       : base($"An organization named '{name}' already exists.")
    {
    }
}