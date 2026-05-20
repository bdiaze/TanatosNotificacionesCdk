using Amazon.APIGateway;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SimpleSystemsManagement;
using LambdaNotificacion.Helpers;
using LambdaNotificacion.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaNotificacion;

public class Function
{
    private readonly IServiceProvider serviceProvider;

    public Function() {
        IHostBuilder builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices((context, services) => {
			#region Singleton AWS Services
			services.AddSingleton<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>();
			services.AddSingleton<IAmazonSecretsManager, AmazonSecretsManagerClient>();
			#endregion

			#region Singleton Helpers
			services.AddSingleton<VariableEntornoHelper>();
			services.AddSingleton<ParameterStoreHelper>();
			services.AddSingleton<SecretManagerHelper>();
			services.AddSingleton<ClientCredentialsHelper>();
			#endregion

			#region Singleton Repositories
			services.AddSingleton<NormaSuscritaDao>();
			#endregion
		});

		IHost app = builder.Build();

		serviceProvider = app.Services;
	}
    

    public async Task FunctionHandler(JsonElement input, ILambdaContext context) {
		Stopwatch stopwatch = Stopwatch.StartNew();

		string rawJson = input.GetRawText();

        LambdaLogger.Log(
			$"[Function] - [FunctionHandler] - " +
			$"Se inicia proceso de envio de notificaciones con parámetros: {rawJson}.");

		NormaSuscritaDao normaSuscritaDao = serviceProvider.GetRequiredService<NormaSuscritaDao>();
		await normaSuscritaDao.ProcesarNotificacion(rawJson);

		LambdaLogger.Log(
			$"[Function] - [FunctionHandler] - [{stopwatch.ElapsedMilliseconds} ms] - " +
			$"Se terminan de procesar las notificaciones exitosamente.");
	}
}
