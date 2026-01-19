using LambdaNotificacion.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.NotificacionBuilders {
	internal class RecordatorioPrevioBuilder(IHostEnvironment env) : INotificacionBuilder {
		public async Task<InformacionNotificacion> ObtenerInformacionNotificacion() {
			return new InformacionNotificacion {
				Asunto = "Recordatorio: Evento próximo",
				Cuerpo = "Este es un recordatorio de que tienes un evento programado próximamente. Por favor, asegúrate de estar preparado."
			};
		}
	}
}
