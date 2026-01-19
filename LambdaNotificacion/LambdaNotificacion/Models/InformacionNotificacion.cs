using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Models {
	internal class InformacionNotificacion {
		public required string Asunto { get; set; }
		public required string Cuerpo { get; set; }
	}
}
