using ClosedXML.Excel;
using AntiScam.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AntiScam.Infrastructure.Services;

public interface IReportExportService
{
    Task<byte[]> ExportScamStatisticsToExcelAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<byte[]> ExportScamStatisticsToPdfAsync(DateTime from, DateTime to, CancellationToken ct = default);
}

public class ReportExportService : IReportExportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportExportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> ExportScamStatisticsToExcelAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var reports = await _unitOfWork.PhoneReports.Query()
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
            .OrderByDescending(r => r.RiskScore)
            .ToListAsync(ct);

        var scenarios = await _unitOfWork.ScamScenarios.Query()
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .ToListAsync(ct);

        using var workbook = new XLWorkbook();

        // Sheet 1: Tổng quan
        var summarySheet = workbook.Worksheets.Add("TongQuan");
        summarySheet.Cell(1, 1).Value = "BÁO CÁO THỐNG KÊ LỪA ĐẢO & AN TOÀN";
        summarySheet.Cell(2, 1).Value = $"Thời gian: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}";
        summarySheet.Cell(4, 1).Value = "Chỉ số";
        summarySheet.Cell(4, 2).Value = "Giá trị";
        summarySheet.Cell(5, 1).Value = "Tổng số kịch bản ghi nhận";
        summarySheet.Cell(5, 2).Value = scenarios.Count;
        summarySheet.Cell(6, 1).Value = "Tổng số SĐT/STK bị báo cáo";
        summarySheet.Cell(6, 2).Value = reports.Count;
        summarySheet.Cell(7, 1).Value = "Số điện thoại nguy cơ cao (Risk > 50)";
        summarySheet.Cell(7, 2).Value = reports.Count(r => r.RiskScore >= 50);

        // Sheet 2: Danh sách SĐT bị báo cáo
        var phoneSheet = workbook.Worksheets.Add("DanhSachBaoCao");
        phoneSheet.Cell(1, 1).Value = "Số/Tài khoản";
        phoneSheet.Cell(1, 2).Value = "Loại";
        phoneSheet.Cell(1, 3).Value = "Điểm rủi ro";
        phoneSheet.Cell(1, 4).Value = "Số lượt báo cáo";
        phoneSheet.Cell(1, 5).Value = "Trạng thái xác minh";
        phoneSheet.Cell(1, 6).Value = "Ngày tạo";

        int row = 2;
        foreach (var r in reports)
        {
            phoneSheet.Cell(row, 1).Value = r.ReportedNumber;
            phoneSheet.Cell(row, 2).Value = r.NumberType.ToString();
            phoneSheet.Cell(row, 3).Value = r.RiskScore;
            phoneSheet.Cell(row, 4).Value = r.ReportCount;
            phoneSheet.Cell(row, 5).Value = r.VerificationStatus.ToString();
            phoneSheet.Cell(row, 6).Value = r.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportScamStatisticsToPdfAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var reports = await _unitOfWork.PhoneReports.Query()
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
            .OrderByDescending(r => r.RiskScore)
            .Take(50)
            .ToListAsync(ct);

        var scenariosCount = await _unitOfWork.ScamScenarios.Query()
            .CountAsync(s => s.CreatedAt >= from && s.CreatedAt <= to, ct);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text("HỆ THỐNG CẢNH BÁO LỪA ĐẢO & AN TOÀN DU LỊCH")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text($"BÁO CÁO TỔNG HỢP NGUY CƠ LỪA ĐẢO ({from:dd/MM/yyyy} - {to:dd/MM/yyyy})").Bold().FontSize(14);
                    col.Item().Text($"• Tổng kịch bản mới ghi nhận: {scenariosCount}");
                    col.Item().Text($"• Tổng số điện thoại/tài khoản khả nghi: {reports.Count}");
                    col.Item().Text("Danh sách các số có điểm rủi ro cao nhất:").Bold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Số / STK").Bold();
                            header.Cell().Text("Loại").Bold();
                            header.Cell().Text("Điểm").Bold();
                            header.Cell().Text("Trạng thái").Bold();
                        });

                        foreach (var item in reports)
                        {
                            table.Cell().Text(item.ReportedNumber);
                            table.Cell().Text(item.NumberType.ToString());
                            table.Cell().Text(item.RiskScore.ToString("F1"));
                            table.Cell().Text(item.VerificationStatus.ToString());
                        }
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
