# Caso de negocio: Taller Mecánico

Un taller mecánico necesita centralizar el registro de clientes, vehículos, mecánicos, especialidades, servicios, repuestos, órdenes de trabajo y pagos. Actualmente, el uso de registros manuales dificulta conocer el estado de una reparación y el historial de atención de cada vehículo.

La API permite al personal autorizado crear y administrar órdenes de trabajo, asociarlas a clientes, vehículos y mecánicos, y registrar los pagos realizados. Los administradores gestionan los datos maestros; los mecánicos pueden consultar y operar sobre clientes y órdenes según sus permisos.

El recurso principal es la orden de trabajo, porque reúne al cliente, vehículo y mecánico responsables de una reparación. La solución expone operaciones REST para administrar el recurso y devuelve códigos HTTP que representan cada resultado.

> Pendiente para la entrega: presentar este documento y registrar la validación de la docente antes de la exposición final.
