namespace Frontend.Api.Stag;

using System.Net;
using Model.Request;
using Model.Response;

class SubjectHandler : ApiHandler {
    public SubjectHandler(string? auth = null) : base(new Uri("http://backend:80/Subject/"), auth) {

    }

    public async Task<IEnumerable<SubjectGetResponse>?> Get() {
        var response = await Get("Get");

        return await response.Content.ReadFromJsonAsync<IEnumerable<SubjectGetResponse>>();
    }

    public async Task Set(int id, SubjectSetRequest request) {
        await Post<SubjectSetRequest>($"{id}/Set", request);
    }
}