using System.Net;

namespace ApiGateway.Handlers
{
    public class HeadersHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var myHeader = request.Headers.FirstOrDefault(c => c.Key == "KeyApi");

            //if (myHeader.Value != null && ((string[])myHeader.Value)[0] == "2")
            //{
            return await base.SendAsync(request, cancellationToken);
            //}

            //var response = new HttpResponseMessage(HttpStatusCode.BadGateway);
            //response.ReasonPhrase = "Your header is not valid";
            //return await Task.FromResult<HttpResponseMessage>(response);

        }
    }
}
