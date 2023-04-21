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

    public async Task AddTime(int subjectId, int eventId, IEnumerable<SubjectSetTimeRequest> times) {
        await Post<IEnumerable<SubjectSetTimeRequest>>($"{subjectId}/TimetableEvent/{eventId}/CreateTimes", times);
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

    public async Task AddLectureEvent(int subjectId) {
        await Put<int>($"{subjectId}/CreateLectureTimetable", 1);
    }

    public async Task AddPractiseEvent(int subjectId) {
        await Put<int>($"{subjectId}/CreatePracticeTimetable", 1);
    }

    public async Task<IEnumerable<TimetableEventGetResponse>> GetTimetableEvents(int subjectId) {
        var response = await Get($"{subjectId}/GetTimetableEvents");
        return await response.Content.ReadFromJsonAsync<IEnumerable<TimetableEventGetResponse>>() ?? new List<TimetableEventGetResponse>();
    }

    public async Task DeleteTimetableEvent(int subjectId, int eventId) {
        var response = await Delete($"{subjectId}/TimetableEvent/{eventId}");
    }

    public async Task DeleteTimetableEventTime(int subjectId, int eventId, int eventTimeId) {
        var response = await Delete($"{subjectId}/TimetableEvent/{eventId}/Time/{eventTimeId}");
    }

    public async Task DeleteSubject(int subjectId) {
        var response = await Delete($"{subjectId}");
    }

    public async Task<IEnumerable<SubjectRelationGetResponse>> GetSubjectRelations(int subjectId) {
        var response = await Get($"{subjectId}/GetSubjectRelations");
        return await response.Content.ReadFromJsonAsync<IEnumerable<SubjectRelationGetResponse>>() ?? new List<SubjectRelationGetResponse>();
    }

    public async Task<IEnumerable<TimetableEventTimeGetResponse>> GetTimetableEventTimes(int subjectId, int eventId) {
        var response = await Get($"{subjectId}/TimetableTimes/{eventId}");
        return await response.Content.ReadFromJsonAsync<IEnumerable<TimetableEventTimeGetResponse>>() ?? new List<TimetableEventTimeGetResponse>();
    }
}