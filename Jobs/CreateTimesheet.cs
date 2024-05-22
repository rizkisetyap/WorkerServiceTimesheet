using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Globalization;
using System.Security;
using WorkerServiceTimesheet.ViewModels;
using WorkerServiceTimesheet.Extensions;

namespace WorkerServiceTimesheet.Jobs
{

    public class TanggalMerah
    {
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
    }
    public class CreateTimesheet : IJob
    {
        private readonly ILogger<CreateTimesheet> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public CreateTimesheet(ILogger<CreateTimesheet> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string rootDir = "D:\\publish\\workerservice\\files";
            var templateFilePath = Path.Combine(rootDir, "Absen-Rizki Setya Pambudi April 2024.xlsx");
            var dateNow = DateTime.Now;
            _logger.LogInformation($"Create Timesheet running at {dateNow}", DateTime.Now);
            int year = dateNow.Year;
            int month = dateNow.Month;
            var daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime firstDateOfMonth = new DateTime(year, month, 1);

            var dataReport = await ReadJsonFile.OpenAndParseJson<ReportActivity>("D:\\absen-sdd\\K-1721.json");
            var activity = dataReport.data.datalist;
            // parse timestamps
            Func<Datalist, ActivityTimesheet> ParseTimestapms = (data) =>
            {
                var checkin = data.checkin;
                var checkout = data.checkout;
                var act = new ActivityTimesheet();
                act.Checkin = ReadJsonFile.ParseUnixTimestamps(checkin);
                act.Checkout = ReadJsonFile.ParseUnixTimestamps(checkout);

                return act;
            };

            var dataHadir = activity.Select(x => ParseTimestapms(x)).Where(x => x.Checkin != null);

            using (var package = new ExcelPackage(templateFilePath))
            {
                var worksheet = package.Workbook.Worksheets.First();
                // Extract Tanggal Merah
                var sheet_tanggal_merah = package.Workbook.Worksheets.Last();
                int second_sheet_row = sheet_tanggal_merah.Dimension.Rows;
                List<TanggalMerah> tanggalMerahs = new List<TanggalMerah>();

                for (int i = 1; i <= second_sheet_row; i++)
                {
                    if (i > 1)
                    {
                        var tanggal = new TanggalMerah();
                        var value_tanggal = Convert.ToDateTime(sheet_tanggal_merah.Cells[i, 2].Value);
                        var value_keterangan = sheet_tanggal_merah.Cells[i, 3].Value;


                        tanggal.Tanggal = value_tanggal;
                        tanggal.Keterangan = value_keterangan.ToString();

                        tanggalMerahs.Add(tanggal);
                    }
                }
                // Bulan Timesheet
                worksheet.Cells[6, 6, 6, 7].Value = dateNow.ToString("MMMM-yy");
                int first_row_ofdate = 10;
                for (int i = 0; i < daysInMonth; i++)
                {
                    DateTime currentDate = firstDateOfMonth.AddDays(i);
                    worksheet.Cells[first_row_ofdate, 2].Value = currentDate.ToString("dd/MM/yyyy");
                    var cellsKeterangan = worksheet.Cells[first_row_ofdate, 13, first_row_ofdate, 14];
                    var cellLibur = worksheet.Cells[first_row_ofdate, 7];
                    var cellHadir = worksheet.Cells[first_row_ofdate, 6];
                    var cell_checkin = worksheet.Cells[first_row_ofdate, 3];
                    var cell_checkout = worksheet.Cells[first_row_ofdate, 4];

                    var date_absen = dataHadir.Where(x => x.Checkin.Value.Date == currentDate.Date).FirstOrDefault();




                    bool isTanggalMerah = tanggalMerahs.Any(x => x.Tanggal.Date == currentDate.Date);

                    if (isTanggalMerah)
                    {
                        cellsKeterangan.Value = tanggalMerahs.Where(x => x.Tanggal.Date == currentDate.Date).FirstOrDefault().Keterangan;
                        cellLibur.Value = "L";

                    }
                    else
                    {
                        if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                        {
                            cellsKeterangan.Value = "Minggu";
                            cellLibur.Value = "L";
                        }
                        else if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                        {
                            cellsKeterangan.Value = "Sabtu";
                            cellLibur.Value = "L";
                        }
                        else
                        {
                            cellHadir.Value = "H";
                            if (date_absen != null)
                            {
                                cell_checkin.Value = date_absen.Checkin != null ? date_absen.Checkin.Value.ToString("HH:mm") : "";
                                cell_checkout.Value = date_absen.Checkout != null ? date_absen.Checkout.Value.ToString("HH:mm") : "";
                            }
                        }
                    }





                    first_row_ofdate += 1;

                }

                worksheet.Cells[48, 4, 48, 6].Value = new DateTime(dateNow.Year, dateNow.Month, 25).ToString("dd/MM/yyyy");
                worksheet.Cells[48, 8, 48, 10].Value = new DateTime(dateNow.Year, dateNow.Month, 25).ToString("dd/MM/yyyy");
                worksheet.Cells[48, 12, 48, 13].Value = new DateTime(dateNow.Year, dateNow.Month, 25).ToString("dd/MM/yyyy");

                var rootPathTS = "C:\\Users\\user\\Desktop\\Timesheet";
                var newDir = Path.Combine(rootPathTS, dateNow.ToString("MMMM yyyy"));
                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }
                var fileName = $"Absen-Rizki Setya Pambudi {dateNow.ToString("MMMM yyyy")}.xlsx";
                FileInfo newFile = new FileInfo(Path.Join(newDir, fileName));
                package.SaveAs(newFile);
            }
            // Your actual work logic here


            await Task.CompletedTask; // Replace with your actual work

        }
    }
}
