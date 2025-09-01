using Domain.Common;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Paging;

namespace Application.Extensiones;

public static class TransactionFilterDtoExtension
{
    public static (List<FilterCriterion> filters,
        Func<IQueryable<Transaction>, IQueryable<Transaction>> orderBy)
        ToRepositoryParams(this TransactionFilterDto filter)
    {
        var list = new List<FilterCriterion>();
        if (list == null) throw new ArgumentNullException(nameof(list));

        if (filter.Type.HasValue)
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Type),
                Operator = FilterOperator.Equals,
                Value = filter.Type.Value
            });

        if (filter.DateFrom != null && filter.DateFrom.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Timestamp),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.DateFrom.Value
            });

        if (filter.DateTo != null && filter.DateTo.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Timestamp),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.DateTo.Value
            });

        if (filter.Reason.HasValue)
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Reason),
                Operator = FilterOperator.Equals,
                Value = filter.Reason.Value
            });

        if (filter.FromAmount.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Amount),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.FromAmount.Value
            });
        }

        if (filter.ToAmount.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(Transaction.Amount),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.ToAmount.Value
            });
        }

        bool asc = filter.OrderDir.Equals("asc", StringComparison.OrdinalIgnoreCase);

        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> order = q =>
            filter.OrderBy.ToLowerInvariant() switch
            {

                "createDate" => asc
                    ? q.OrderBy(n => n.Timestamp)
                    : q.OrderByDescending(n => n.Timestamp),

                "type" => asc
                    ? q.OrderBy(n => n.Type)
                    : q.OrderByDescending(n => n.Type),

                "reason" => asc
                    ? q.OrderBy(n => n.Reason)
                    : q.OrderByDescending(n => n.Reason),

                "amount" => asc
                    ? q.OrderBy(n => n.Amount)
                    : q.OrderByDescending(n => n.Amount),

                _ => asc
                    ? q.OrderBy(n => n.Id)
                    : q.OrderByDescending(n => n.Id),
            };

        return (list, order);
    }
}