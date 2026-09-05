using FluentValidation;
using TallerMecanico.Models;

namespace TallerMecanico.Validators
{
    public class OrdenesTrabajoValidator : AbstractValidator<OrdenesTrabajo>
    {
        public OrdenesTrabajoValidator()
        {
            RuleFor(x => x.Estado).NotEmpty().WithMessage("El estado de la orden es obligatorio.");
            RuleFor(x => x.ClienteId).GreaterThan(0).WithMessage("Debe seleccionar un cliente válido.");
            RuleFor(x => x.VehiculoId).GreaterThan(0).WithMessage("Debe seleccionar un vehículo válido.");
            RuleFor(x => x.MecanicoId).GreaterThan(0).WithMessage("Debe seleccionar un mecánico válido.");
        }
    }
}
