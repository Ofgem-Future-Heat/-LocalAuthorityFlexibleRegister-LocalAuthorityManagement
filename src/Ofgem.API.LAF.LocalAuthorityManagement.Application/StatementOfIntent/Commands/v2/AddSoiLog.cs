using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Application.StatementOfIntent.Commands.v2
{
    public class AddSoiLog : IRequest<Result<string>>
    {
        public Ofgem.LAF.SharedLibrary.Models.StatementOfIntentLog StatementOfIntentLog { get; set; }
    }
}
