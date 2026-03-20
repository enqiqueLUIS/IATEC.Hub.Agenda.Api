using System.Net;
using System.Text.Json.Serialization;
using Agenda.Core.Entities.Core.CustomEntities.ResponseApi.Details;

namespace Agenda.Core.Entities.Core.ResponseApi;

public class ResponsePost
{
    public Message[] Messages { get; set; }
    public int Id { get; set; }
    public ResponsePostDetail Response { get; set; }
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
}