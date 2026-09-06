using System;
using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record PagoDTO(
        [property: Range(typeof(int), "1", "2147483647", ErrorMessage = "Debe asignar un ID de orden válido.")] int OrdenId,
        [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "El monto del pago debe ser mayor a 0.")] decimal MontoTotal,
        [property: Required(ErrorMessage = "El método de pago es obligatorio.")] string MetodoPago,
        DateTime? FechaPago,
        string Estado);
}
