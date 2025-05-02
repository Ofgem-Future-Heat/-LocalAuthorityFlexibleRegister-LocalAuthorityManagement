using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2
{
    public class SoiStatusSetting : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public Ofgem.LAF.SharedLibrary.Models.StatementOfIntent StatementOfIntent;
    }
}
