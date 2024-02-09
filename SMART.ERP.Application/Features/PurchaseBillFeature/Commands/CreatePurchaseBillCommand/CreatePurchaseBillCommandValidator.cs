using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand
{
    public class CreatePurchaseBillCommandValidator : AbstractValidator<CreatePurchaseBillCommand>
    {
        public CreatePurchaseBillCommandValidator() {
        }
    }
}
