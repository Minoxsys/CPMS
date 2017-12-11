using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using CPMS.Authorization;
using CPMS.Authorization.WebAPI;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;
using CPMS.Report.Rendering.Adapters;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    [AuthorizeByQueryStringAttribute(RequiresPermission = PermissionId.MonthlyRTTPeriodPerformance)]
    public class Monthly18wRTTPerformanceFileController : ApiController
    {
        private readonly ReportService _reportService;

        public Monthly18wRTTPerformanceFileController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public HttpResponseMessage GetMonthly18wRTTPerformanceReportFile([ModelBinder(typeof(RoleDataModelBinder))]RoleData role, DateTime fromDate, DateTime toDate,
            Format format = Format.Excel, int hospitalId = 0, string specialtyCode = null, int clinicianId = 0, Granularity granularity = Granularity.Hospital)
        {
            var reportFile = _reportService.GetMonthly18wRTTPerformanceReportFile(role, fromDate, toDate, format, hospitalId, specialtyCode,
                clinicianId, granularity);

            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(reportFile.Content)};

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = reportFile.FileName
            };

            return response;
        }
    }
}
