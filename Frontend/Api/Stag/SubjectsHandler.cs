namespace Frontend.Api.Stag;

using System.Net;
using Model.Request;
using Model.Response;

class SubjectHandler : ApiHandler {
    public SubjectHandler(string? auth = null) : base("Subject/", auth) {

    }

    public async Task<IEnumerable<SubjectGetResponse>?> Get() {
        var response = await Get("Get");

        return await response.Content.ReadFromJsonAsync<IEnumerable<SubjectGetResponse>>();
    }

    public async Task Set(int id, SubjectSetRequest request) {
        await Post<SubjectSetRequest>($"{id}/Set", request);
    }

    public async Task SetGarant(int subjectId, String userId) {
        await Get($"{subjectId}/SetGarant/{userId}");
    }

    public async Task SetPracticioner(int subjectId, String userId) {
        await Get($"{subjectId}/SetPracticioner/{userId}");
    }

    public async Task SetTutor(int subjectId, String userId) {
        await Get($"{subjectId}/SetTutor/{userId}");
    }

    public async Task<IEnumerable<TimetableEventGetResponse>> GetTimetableEvents(int subjectId) {
        var response = await Get($"{subjectId}/GetTimetableEvents");
        return await response.Content.ReadFromJsonAsync<IEnumerable<TimetableEventGetResponse>>() ?? new List<TimetableEventGetResponse>();
    }

    public async Task<IEnumerable<SubjectRelationGetResponse>> GetSubjectRelations(int subjectId) {
        var response = await Get($"{subjectId}/GetSubjectRelations");
        return await response.Content.ReadFromJsonAsync<IEnumerable<SubjectRelationGetResponse>>() ?? new List<SubjectRelationGetResponse>();
    }
}