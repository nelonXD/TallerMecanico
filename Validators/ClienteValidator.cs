using FluentValidation;
using TallerMecanico.Models;

namespace TallerMecanico.Validators
{
    public class ClienteValidator : AbstractValidator<Cliente>
    {
        public ClienteValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.").MaximumLength(100);
            RuleFor(x => x.Apellido).NotEmpty().WithMessage("El apellido es obligatorio.").MaximumLength(100);
            RuleFor(x => x.Correo).NotEmpty().EmailAddress().WithMessage("Debe proporcionar un correo electrónico válido.");
            RuleFor(x => x.Telefono).NotEmpty().WithMessage("El teléfono es obligatorio.");
        }
    }
}
