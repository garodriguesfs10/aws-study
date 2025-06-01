using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace lambda_demo;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var name = "stranger";
        if (request != null)
        {
            if (request.QueryStringParameters != null)
            {
                request.QueryStringParameters.TryGetValue("name", out name);
                var message = $"Hi {name}, from AWS Lambda - You used query parameter";
                return message;
            }

            if (request.PathParameters != null)
            {
                request.PathParameters.TryGetValue("name", out name);
                var message = $"Hi {name}, from AWS Lambda - You used path parameter";
                return message;
            }
        }

        return "Hey stranger, use some query or path parameters";
    }
}
