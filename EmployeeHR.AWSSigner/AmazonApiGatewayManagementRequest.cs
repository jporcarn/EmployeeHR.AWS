using Amazon.APIGateway;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using System;
using System.Linq;
using System.Net.Http;



/// <summary>
/// 
/// </summary>
/// <see cref="https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Core/Amazon.Runtime/Internal/DefaultRequest.cs"/>
namespace EmployeeHR.AWSSigner
{
    public class AmazonApiGatewayManagementRequest : DefaultRequest
    {

        public AmazonApiGatewayManagementRequest(HttpRequestMessage request) : base(new AmazonAPIGatewayRequest(), "execute-api")
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // this.UseSigV4 = true;
            this.SignatureVersion = SignatureVersion.SigV4;
            this.HttpMethod = request.Method.Method;
            this.Endpoint = new Uri(request.RequestUri.AbsoluteUri[..request.RequestUri.AbsoluteUri.LastIndexOf(request.RequestUri.AbsolutePath)]);
            this.ResourcePath = request.RequestUri.AbsolutePath;

            if (request.Content != null)
            {
                this.Content = request.Content.ReadAsByteArrayAsync().Result;
                this.ContentStream = request.Content.ReadAsStreamAsync().Result;
            }

            var headers = request.Headers.ToDictionary(e => e.Key, e => e.Value.FirstOrDefault());

            headers.ToList().ForEach(h =>
            {
                this.Headers.Add(h);
            });

        }

    }
}