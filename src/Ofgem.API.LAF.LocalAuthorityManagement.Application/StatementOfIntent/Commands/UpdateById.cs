using MediatR;
using Ofgem.LAF.SharedLibrary.Enums;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands;

public class UpdateById : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent>>
{
    public Ofgem.LAF.SharedLibrary.Models.StatementOfIntent StatementOfIntent { get; set; }
}