using MediatR;
using Ofgem.API.LAF.LocalAuthorityManagement.Application.Abstractions;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.LocalAuthority.CommandHandlers;

public class UpdateByOnsCodeHandler(ILocalAuthorityRepository localAuthorityRepository)
    : IRequestHandler<Commands.UpdateByOnsCode, Ofgem.LAF.SharedLibrary.Models.LocalAuthority?>
{
    public async Task<Ofgem.LAF.SharedLibrary.Models.LocalAuthority?> Handle(Commands.UpdateByOnsCode request, CancellationToken cancellationToken)
    {
        var localAuthority = await localAuthorityRepository.UpdateByOnsCode(
            request.OnsCode!,
            request.Name!,
            request.Email!);
        return localAuthority;
    }
}