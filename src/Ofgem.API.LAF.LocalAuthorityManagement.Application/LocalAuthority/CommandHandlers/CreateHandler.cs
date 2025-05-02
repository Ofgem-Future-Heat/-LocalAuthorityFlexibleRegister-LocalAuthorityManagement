using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.Commands;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.CommandHandlers;

public class CreateHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Create, Ofgem.LAF.SharedLibrary.Models.LocalAuthority>
{
    public async Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority> Handle(Create request, CancellationToken cancellationToken)
    {
        var localAuthority = new Ofgem.LAF.SharedLibrary.Models.LocalAuthority
        {
            Name = request.Name,
            OnsCode = request.OnsCode!,
            Email = request.Email
        };

        return await localAuthorityRepository.Create(localAuthority);
    }
}