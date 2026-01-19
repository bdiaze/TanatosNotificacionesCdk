using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Models {
	internal class HermesCorreo {
		public DireccionCorreo? De { get; set; }
		public required List<DireccionCorreo> Para { get; set; }
		public List<DireccionCorreo>? Cc { get; set; }
		public List<DireccionCorreo>? Cco { get; set; }
		public List<DireccionCorreo>? ResponderA { get; set; }
		public required string Asunto { get; set; }
		public required string Cuerpo { get; set; }
		public List<Adjunto>? Adjuntos { get; set; }
	}

	internal class DireccionCorreo {
		public string? Nombre { get; set; }
		public required string Correo { get; set; }
	}

	internal class Adjunto {
		public required string NombreArchivo { get; set; }
		public required string TipoMime { get; set; }
		public required string ContenidoBase64 { get; set; }
	}
}
