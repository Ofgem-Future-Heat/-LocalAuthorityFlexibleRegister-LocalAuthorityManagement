using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.CommandHandlers;

public class UpdateHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Commands.Update, Ofgem.LAF.SharedLibrary.Models.LocalAuthority?>
{
    public async Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> Handle(Commands.Update request, CancellationToken cancellationToken)
    {
        var localAuthority = await localAuthorityRepository.Update(request.Name ?? string.Empty, request.LocalAuthorityId);
        return localAuthority;
    }
}