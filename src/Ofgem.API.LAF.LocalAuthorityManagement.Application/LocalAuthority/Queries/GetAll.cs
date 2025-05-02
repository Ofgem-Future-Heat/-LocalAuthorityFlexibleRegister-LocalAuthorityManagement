using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Queries;

public class GetAll : IRequest<ICollection<Ofgem.LAF.SharedLibrary.Models.LocalAuthority>>
{
    
}