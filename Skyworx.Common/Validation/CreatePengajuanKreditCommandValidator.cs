using FluentValidation;
using Skyworx.Common.Command;

namespace Skyworx.Common.Validation;

public class KreditRequestValidator<T> : AbstractValidator<T> where T : IKreditRequest
{
    public KreditRequestValidator()
    {
        RuleFor(x => x.Plafon)
            .GreaterThan(0).WithMessage("Plafon harus lebih dari 0");

        RuleFor(x => x.Bunga)
            .GreaterThan(0).WithMessage("Bunga harus lebih dari 0")
            .LessThanOrEqualTo(100).WithMessage("Bunga tidak boleh lebih dari 100");

        RuleFor(x => x.Tenor)
            .GreaterThan(0).WithMessage("Tenor harus lebih dari 0");
    }
}