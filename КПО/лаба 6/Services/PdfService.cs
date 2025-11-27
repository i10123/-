using Hospital.Domain.Entities;
using Hospital.Domain.Entities.Base;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Hospital.Services
{
    public static class PdfService
    {
        public static void GenerateEpicrisis(Patient patient, User doctor, string filePath)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // --- ШАПКА ---
                    page.Header().Text("ВЫПИСНОЙ ЭПИКРИЗ").SemiBold().FontSize(20).AlignCenter();

                    // --- КОНТЕНТ ---
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        // Данные пациента
                        column.Item().Text($"ФИО Пациента: {patient.FullName}").Bold();
                        column.Item().Text($"Дата рождения: {patient.BirthDate:dd.MM.yyyy}");
                        column.Item().Text($"Адрес: {patient.Address}");

                        var startDate = patient.MedicalRecord.AdmissionDate.ToString("dd.MM.yyyy");
                        var endDate = patient.MedicalRecord.DischargeDate.HasValue
                                      ? patient.MedicalRecord.DischargeDate.Value.ToString("dd.MM.yyyy")
                                      : System.DateTime.Now.ToString("dd.MM.yyyy");

                        column.Item().Text($"Период лечения: {startDate} — {endDate}");

                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Диагноз
                        column.Item().Text("Диагноз:").Bold();
                        column.Item().Text(patient.MedicalRecord.Diagnosis);

                        column.Item().PaddingTop(10).Text("Анамнез:").Bold();
                        column.Item().Text(patient.MedicalRecord.Anamnesis);

                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Таблица назначений
                        column.Item().Text("Выполненные назначения:").Bold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            // Определение колонок
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Дата
                                columns.RelativeColumn(3); // Название
                                columns.RelativeColumn(2); // Врач
                            });

                            // Заголовки
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Дата");
                                header.Cell().Element(CellStyle).Text("Назначение");
                                header.Cell().Element(CellStyle).Text("Врач");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Строки
                            foreach (var treat in patient.MedicalRecord.Treatments)
                            {
                                table.Cell().Element(CellStyle).Text($"{treat.DatePrescribed:dd.MM}");
                                table.Cell().Element(CellStyle).Text(treat.InfoText);
                                table.Cell().Element(CellStyle).Text(treat.DoctorName);

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                    });

                    // --- ПОДВАЛ ---
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Лечащий врач: ");
                            x.Span(doctor.FullName).Bold();
                            x.Span($"   |   Дата формирования: {DateTime.Now:dd.MM.yyyy}");
                        });
                });
            })
            .GeneratePdf(filePath); // Сохраняем файл
        }
    }
}