using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class GetAssociatedLocalAuthorities : IRequest<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    // this is the name if the base local authority
    // we will get the names of other local authorities that this 
    // local authority can handle
    public string? OnsCode { get; init; }
}