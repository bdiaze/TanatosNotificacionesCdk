using LambdaNotificacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.NotificacionBuilders {
	internal interface INotificacionBuilder {
		Task<InformacionNotificacion> ObtenerInformacionNotificacion();
	}
}
