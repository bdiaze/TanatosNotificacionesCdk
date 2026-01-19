using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Models {
	
	public class EntradaLambda {
		public required long IdNormaSuscrita { get; set; }
		public required bool ProgramarSiguienteEjecucion { get; set; }
	}
}
