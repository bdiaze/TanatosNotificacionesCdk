using LambdaNotificacion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Helpers {
	internal class HermesHelper(VariableEntornoHelper variableEntorno, ParameterStoreHelper parameterStore, ApiKeyHelper apiKey) {
		private readonly string _baseUrl = parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_HERMES_API_URL")).Result;
		private readonly string _xApiKey = apiKey.ObtenerApiKey(parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_HERMES_API_KEY_ID")).Result).Result;

		private readonly string _deNombre = variableEntorno.Obtener("HERMES_DE_NOMBRE");
		private readonly string _deCorreo = variableEntorno.Obtener("HERMES_DE_CORREO");

		public async Task<HermesRetorno> EnviarCorreo(HermesCorreo correo) {
			correo.De ??= new DireccionCorreo { 
				Nombre = _deNombre,
				Correo = _deCorreo
			};

			using HttpClient client = new(new RetryHandler(new HttpClientHandler()));
			client.DefaultRequestHeaders.Add("x-api-key", _xApiKey);
			HttpResponseMessage response = await client.PostAsync(_baseUrl + "Correo/Enviar", new StringContent(JsonConvert.SerializeObject(correo), Encoding.UTF8, "application/json"));
			return JsonConvert.DeserializeObject<HermesRetorno>(await response.Content.ReadAsStringAsync())!;
		}
	}
}
