using Domain.Common;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Paging;

namespace Application.Extensiones;

public static class PaymentRequestFilterDtoExtension
{
    public static (List<FilterCriterion> filters,
        Func<IQueryable<PaymentRequest>, IQueryable<PaymentRequest>> orderBy)
        ToRepositoryParams(this PaymentRequestFilterDto filter)
    {
        var list = new List<FilterCriterion>();
        if (list == null) throw new ArgumentNullException(nameof(list));
        
        if (filter.Status.HasValue)
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.Status),
                Operator = FilterOperator.Equals,
                Value = filter.Status.Value
            });
        
        if (filter.Method.HasValue)
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.Method),
                Operator = FilterOperator.Equals,
                Value = filter.Method.Value
            });
        
        if (filter.DateFrom != null && filter.DateFrom.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.CreateDate),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.DateFrom.Value
            });

        if (filter.DateTo != null && filter.DateTo.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.CreateDate),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.DateTo.Value
            });
        
        if (filter.FromAmount.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.Amount),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.FromAmount.Value
            });
        }

        if (filter.ToAmount.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(PaymentRequest.Amount),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.ToAmount.Value
            });
        }
        
        bool asc = filter.OrderDir.Equals("asc", StringComparison.OrdinalIgnoreCase);

        Func<IQueryable<PaymentRequest>, IOrderedQueryable<PaymentRequest>> order = q =>
            filter.OrderBy.ToLowerInvariant() switch
            {
                "method" => asc
                    ? q.OrderBy(n => n.Method)
                    : q.OrderByDescending(n => n.Method),

                "createDate" => asc
                    ? q.OrderBy(n => n.CreateDate)
                    : q.OrderByDescending(n => n.CreateDate),

                "status" => asc
                    ? q.OrderBy(n => n.Status)
                    : q.OrderByDescending(n => n.Status),

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