namespace Frontend.Api.Stag;

using System.Net;
using Model.Request;
using Model.Response;

class DepartmentHandler : ApiHandler {
    public DepartmentHandler(string? auth = null) : base("Department/", auth) {

    }

    public async Task<SubjectPutResponse> CreateSubject(int departmentId, SubjectPutRequest request) {
        var response = await Put($"{departmentId}/CreateSubject", request);
        return await response.Content.ReadFromJsonAsync<SubjectPutResponse>();
    }
}