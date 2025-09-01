using Domain.Common;
using Domain.DTOs.Support;
using Domain.Entities;
using Domain.Enumes.Paging;

namespace Application.Extensiones;

public static class SupportTicketFilterDtoExtension
{
    public static (List<FilterCriterion> filters,
        Func<IQueryable<SupportTicket>, IOrderedQueryable<SupportTicket>> orderBy)
        ToRepositoryParams(this SupportTicketFilterDto filter)
    {
        var list = new List<FilterCriterion>();

        if (filter.Status.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(SupportTicket.Status),
                Operator     = FilterOperator.Equals,
                Value        = filter.Status.Value
            });
        }

        if (filter.AssignedAdminId.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(SupportTicket.AssignedAdminId),
                Operator     = FilterOperator.Equals,
                Value        = filter.AssignedAdminId.Value
            });
        }

        if (!string.IsNullOrWhiteSpace(filter.SubjectContains))
        {
            list.Add(new()
            {
                PropertyName = nameof(SupportTicket.Subject),
                Operator     = FilterOperator.Contains,
                Value        = filter.SubjectContains!
            });
        }

        if (filter.DateFrom is { } df && df != DateTime.MinValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(SupportTicket.CreateDate),
                Operator     = FilterOperator.GreaterThanOrEqual,
                Value        = df
            });
        }

        if (filter.DateTo is { } dt && dt != DateTime.MinValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(SupportTicket.CreateDate),
                Operator     = FilterOperator.LessThanOrEqual,
                Value        = dt
            });
        }

        bool asc = filter.OrderDir.Equals("asc", StringComparison.OrdinalIgnoreCase);

        Func<IQueryable<SupportTicket>, IOrderedQueryable<SupportTicket>> order = q =>
            filter.OrderBy.ToLowerInvariant() switch
            {
                "createdate" => asc
                    ? q.OrderBy(t => t.CreateDate)
                    : q.OrderByDescending(t => t.CreateDate),

                "updatedate" => asc
                    ? q.OrderBy(t => t.ModifiedDate)
                    : q.OrderByDescending(t => t.ModifiedDate),

                "status" => asc
                    ? q.OrderBy(t => t.Status)
                    : q.OrderByDescending(t => t.Status),

                "assigned" => asc
                    ? q.OrderBy(t => t.AssignedAdminId)
                    : q.OrderByDescending(t => t.AssignedAdminId),

                _ => asc
                    ? q.OrderBy(t => t.Id)
                    : q.OrderByDescending(t => t.Id),
            };

        return (list, order);
    }
}
