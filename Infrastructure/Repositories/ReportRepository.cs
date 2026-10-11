using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Enums;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class ReportRepository(IDbContextFactory<ApplicationDbContext> factory) : IReportRepository
{
    public async Task<Report> AddAsync(Report report)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Reports.Add(report);
        await context.SaveChangesAsync();
        return report;
    }

    public async Task<Report?> GetByIdAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Reports.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<Report>> GetPendingAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Reports
            .AsNoTracking()
            .Include(r => r.Reporter)
            .Include(r => r.Post)
                .ThenInclude(p => p.Author)
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasPendingAsync(int postId, string reporterId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Reports
            .AnyAsync(r => r.PostId == postId
            && r.ReporterId == reporterId
            && r.Status == ReportStatus.Pending);
    }

    public async Task UpdateAsync(Report report)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(report).State = EntityState.Modified;
        await context.SaveChangesAsync();

    }
}
