namespace Frontend.Api.Stag;

using System.Net;
using Model.Request;
using Model.Response;

class PersonHandler : ApiHandler {
    public PersonHandler(string? auth = null) : base("Person/", auth) {

    }

    public async Task<IEnumerable<PersonGetResponse>> GetPersons() {
        var response = await Get("GetUsers");

        return await response.Content.ReadFromJsonAsync<IEnumerable<PersonGetResponse>>();
    }

    public async Task<IEnumerable<SubjectGetResponse>?> GetSubjects() {
        var response = await Get("GetSubjects");

        return await response.Content.ReadFromJsonAsync<IEnumerable<SubjectGetResponse>>();
    }

    public async Task<IEnumerable<TimetableEventTimeResponse>> GetTimetableTimes(DateTime from, DateTime to) {
        var queryParameters = new Dictionary<string, string>
        {
            { "from", from.ToUniversalTime().ToString() },
            { "to", to.ToUniversalTime().ToString() }
        };
        var dictFormUrlEncoded = new FormUrlEncodedContent(queryParameters);
        var queryString = await dictFormUrlEncoded.ReadAsStringAsync();
        
        var response = await Get($"GetTimetableTimes?{queryString}");
        return await response.Content.ReadFromJsonAsync<IEnumerable<TimetableEventTimeResponse>>();
    }
}