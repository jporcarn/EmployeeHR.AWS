using Amazon.ApiGatewayManagementApi;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


// aws-signer-v4-using-sdk-net

namespace EmployeeHR.AWSSigner
{
    /// <summary>
    /// AWS Signature Version 4 Http Interceptor
    /// </summary>
    /// <see cref="https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html"/>
    public class AWSSignatureV4HttpInterceptor : DelegatingHandler
    {
        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _region;
        private readonly string? _sessionToken;
        private readonly string _service = "execute-api";

        public AWSSignatureV4HttpInterceptor(string accessKeyId, string secretAccessKey, string region, string? sessionToken = null) : base(new HttpClientHandler())
        {
            this._accessKeyId = accessKeyId;
            this._secretAccessKey = secretAccessKey;
            this._region = region;
            this._sessionToken = sessionToken;
        }

        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            if (_sessionToken != null)
            {
                request.Headers.Add(HeaderKeys.XAmzSecurityTokenHeader, _sessionToken);
            }

            Sign(request, this._accessKeyId, this._secretAccessKey, this._region, this._service);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            return response;
        }


        private void Sign(HttpRequestMessage request, string awsAccessKeyId, string awsSecretAccessKey, string region, string service)
        {

            bool signPayload = true;

            AWS4Signer signer = new AWS4Signer(signPayload);

            var metrics = new RequestMetrics();

            var clientConfig = new AmazonApiGatewayManagementApiConfig();

            IRequest executeApiRequest = new AmazonApiGatewayManagementRequest(request);

            AWS4SigningResult result = signer.SignRequest(executeApiRequest, clientConfig, metrics, awsAccessKeyId, awsSecretAccessKey);

            executeApiRequest.Headers.ToList().ForEach(h =>
            {
                request.Headers.TryAddWithoutValidation(h.Key, h.Value);
            });

            request.Headers.TryAddWithoutValidation("Authorization", result.ForAuthorizationHeader);
        }

    }
}