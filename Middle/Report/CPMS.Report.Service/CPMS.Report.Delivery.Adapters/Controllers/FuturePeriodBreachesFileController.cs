﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using CPMS.Infrastructure.WebApi;
using CPMS.Report.Presentation;
using CPMS.Report.Rendering.Adapters;

namespace CPMS.Report.Delivery.Adapters.Controllers
{
    public class FuturePeriodBreachesFileController : ApiController
    {
        private readonly ReportService _reportService;

        public FuturePeriodBreachesFileController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [ElmahGenericExceptionHandling]
        public HttpResponseMessage GetFuturePeriodBreachesReportFile(int weeksToBreach,
            Format format = Format.Excel, Layout layout = Layout.Tabular, int hospitalId = 0, string specialtyCode = null, int clinicianId = 0, Granularity granularity = Granularity.Hospital, string pathwayType = null)
        {
            var reportFile = _reportService.GetFuturePeriodBreachesReportFile(weeksToBreach, format, layout, hospitalId, specialtyCode,
                clinicianId, granularity, pathwayType);

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
