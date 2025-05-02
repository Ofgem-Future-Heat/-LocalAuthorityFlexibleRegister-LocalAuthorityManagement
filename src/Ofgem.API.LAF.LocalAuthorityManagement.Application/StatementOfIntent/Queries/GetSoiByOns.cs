using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Queries
{
    public class GetSoiByOns() : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public string? OnsCode { get; set; }

        public DateTime PublishedDate { get; set; }
    }
}
