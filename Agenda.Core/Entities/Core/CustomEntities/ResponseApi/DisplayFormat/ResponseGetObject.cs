using System.Net;
using System.Text.Json.Serialization;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;

namespace Agenda.Core.Entities.Core.ResponseApi;

public class ResponseGetObject
{
    public object Data { get; set; }
    public Message[] Messages { get; set; }
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
}