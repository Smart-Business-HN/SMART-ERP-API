using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.UserFeature.Commands.RemoveSessionCommand
{
    public class RemoveSessionCommandValidator : AbstractValidator<RemoveSessionCommand>
    {
        public RemoveSessionCommandValidator()
        {
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} no puede ser nulo");
        }
    }
}
