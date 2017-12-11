using System;
using CPMS.Report.Presentation;
using Microsoft.Reporting.WebForms;

namespace CPMS.Report.Rendering.Adapters
{
    public class ReportService
    {
        private readonly ReportPresentationService _reportPresentationService;

        public ReportService(ReportPresentationService reportPresentationService)
        {
            _reportPresentationService = reportPresentationService;
        }

        public File GetPeriodBreachesReportFile(DateTime fromDate, DateTime toDate,
            Format format, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            string outputFileName;
            string templateFileName;

            switch (granularity)
            {
                case Granularity.Specialty:
                {
                    templateFileName =
                        "CPMS.Report.Rendering.Adapters.Templates.PeriodBreaches_Specialty_Tabular_.rdlc";

                    outputFileName = "Monthly18wRTTPerformanceBySpecialty";
                    break;
                }
                case Granularity.Clinician:
                {
                    templateFileName =
                        "CPMS.Report.Rendering.Adapters.Templates.PeriodBreaches_Clinician_Tabular_.rdlc";

                    outputFileName = "Monthly18wRTTPerformanceByClinician";
                    break;
                }
                case Granularity.Hospital:
                {
                    templateFileName =
                        "CPMS.Report.Rendering.Adapters.Templates.PeriodBreaches_Hospital_Tabular_.rdlc";

                    outputFileName = "Monthly18wRTTPerformanceByHospital";
                    break;
                }

                default:
                    throw new NotSupportedException(string.Format("Granularity {0} is not supported!", granularity));
            }

            var periodBreachesReportData = _reportPresentationService.GetPeriodBreachesReport(fromDate, toDate, hospitalId,
                specialtyCode, clinicianId, granularity);
            var reportDataSource = new ReportDataSource("PeriodBreaches", periodBreachesReportData);

            return GetReportFile(format, templateFileName, outputFileName, reportDataSource);
        }

        public File GetFuturePeriodBreachesReportFile(int weeksToBreach,
            Format format, Layout layout, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            string outputFileName;
            string templateFileName;

            switch (granularity)
            {
                case Granularity.Specialty:
                    switch (layout)
                    {
                        case Layout.BarChart:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Specialty_BarChart.rdlc";
                            break;
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Specialty_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "FuturePeriodBreachesBySpecialty";
                    break;
                case Granularity.Clinician:
                    switch (layout)
                    {
                        case Layout.BarChart:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Clinician_BarChart.rdlc";
                            break;
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Clinician_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "FuturePeriodBreachesByClinician";
                    break;
                case Granularity.Hospital:
                    switch (layout)
                    {
                        case Layout.BarChart:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Hospital_BarChart.rdlc";
                            break;
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.FuturePeriodBreaches_Hospital_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "FuturePeriodBreachesByHospital";
                    break;
                default:
                    throw new NotSupportedException(string.Format("Granularity {0} is not supported!", granularity));
            }

            var futurePeriodBreachesReportData = _reportPresentationService.GetFuturePeriodBreachesReport(weeksToBreach, hospitalId,
                specialtyCode, clinicianId, granularity);
            var reportDataSource = new ReportDataSource("FuturePeriodBreaches", futurePeriodBreachesReportData);

            return GetReportFile(format, templateFileName, outputFileName, reportDataSource);
        }

        public File GetActivePeriodsDistributionReportFile(Format format, Layout layout, int hospitalId, string specialtyCode, int clinicianId, Granularity granularity)
        {
            string outputFileName;
            string templateFileName;

            switch (granularity)
            {
                case Granularity.Specialty:
                    switch (layout)
                    {
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.ActivePeriodsDistribution_Specialty_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "ActivePeriodsDistributionBySpecialty";
                    break;
                case Granularity.Clinician:
                    switch (layout)
                    {
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.ActivePeriodsDistribution_Clinician_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "ActivePeriodsDistributionByClinician";
                    break;
                case Granularity.Hospital:
                    switch (layout)
                    {
                        case Layout.Tabular:
                            templateFileName =
                                "CPMS.Report.Rendering.Adapters.Templates.ActivePeriodsDistribution_Hospital_Tabular.rdlc";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Layout {0} is not supported!", layout));
                    }
                    outputFileName = "ActivePeriodsDistributionByHospital";
                    break;
                default:
                    throw new NotSupportedException(string.Format("Granularity {0} is not supported!", granularity));
            }

            var activePeriodsDistributionReportData = _reportPresentationService.GetActivePeriodsDistributionReport(hospitalId, specialtyCode, clinicianId, granularity);
            var reportDataSource = new ReportDataSource("ActivePeriodsDistribution", activePeriodsDistributionReportData);

            return GetReportFile(format, templateFileName, outputFileName, reportDataSource);
        }

        private File GetReportFile(Format format, string templateFileName, string outputFileName, ReportDataSource reportDataSource)
        {
            using (var localReport = new LocalReport { ReportEmbeddedResource = templateFileName })
            {
                localReport.DataSources.Add(reportDataSource);

                string mimeType;
                string encoding;
                string fileNameExtension;

                Warning[] warnings;
                string[] streams;

                byte[] renderedBytes = localReport.Render(
                    format.ToString(),
                    string.Empty,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                return new File
                {
                    Content = renderedBytes,
                    FileName = string.Format("{0}.{1}", outputFileName, fileNameExtension)
                };
            }
        }
    }
}