using MediatR;
using Ofgem.LAF.SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2
{
    public class SchemeDetails : IRequest<Result<Ofgem.LAF.SharedLibrary.Models.StatementOfIntent?>>
    {
        public Guid StatementOfIntentId { get; set; }

        public ForSchemeEnum ForScheme { get; set; }

        public bool? IsPublishedDateCorrect { get; set; }

        public bool? IsProxy5PartOfRoute2 { get; set; }
    }
}
