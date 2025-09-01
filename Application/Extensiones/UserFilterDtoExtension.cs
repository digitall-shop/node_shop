using Domain.Common;
using Domain.DTOs.User;
using Domain.Entities;
using Domain.Enumes.Paging;

namespace Application.Extensiones;

public static class UserFilterDtoExtension
{
    public static (List<FilterCriterion> filters,
        Func<IQueryable<User>, IQueryable<User>> orderBy)
        ToRepositoryParams(this UserFilterDto filter)
    {
        var list = new List<FilterCriterion>();
        if (list == null) throw new ArgumentNullException(nameof(list));
        
        if (filter.DateFrom != null && filter.DateFrom.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(User.CreateDate),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.DateFrom.Value
            });

        if (filter.DateTo != null && filter.DateTo.Value != DateTime.MinValue)
            list.Add(new()
            {
                PropertyName = nameof(User.CreateDate),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.DateTo.Value
            });
        
        if (filter.FromBalance.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(User.Balance),
                Operator = FilterOperator.GreaterThanOrEqual,
                Value = filter.FromBalance.Value
            });
        }

        if (filter.ToBalance.HasValue)
        {
            list.Add(new()
            {
                PropertyName = nameof(User.Balance),
                Operator = FilterOperator.LessThanOrEqual,
                Value = filter.ToBalance.Value
            });
        }

        if (filter.UserId.HasValue)
        {
            list.Add(new ()
            {
                PropertyName = nameof(User.Id),
                Operator = FilterOperator.Equals,
                Value = filter.UserId.Value
            });
        }

        if (filter.UserName !=null)
        {
            list.Add(new ()
            {
                PropertyName = nameof(User.UserName),
                Operator = FilterOperator.Equals,
                Value = filter.UserName
            });
        }
        bool asc = filter.OrderDir.Equals("asc", StringComparison.OrdinalIgnoreCase);
        
        Func<IQueryable<User>, IOrderedQueryable<User>> order = q =>
            filter.OrderBy.ToLowerInvariant() switch
            {
                "username" => asc
                    ? q.OrderBy(n => n.UserName)
                    : q.OrderByDescending(n => n.UserName),

                "createDate" => asc
                    ? q.OrderBy(n => n.CreateDate)
                    : q.OrderByDescending(n => n.CreateDate),

                "userid" => asc
                    ? q.OrderBy(n => n.Id)
                    : q.OrderByDescending(n => n.Id),

                "balance" => asc
                    ? q.OrderBy(n => n.Balance)
                    : q.OrderByDescending(n => n.Balance),

                _ => asc
                    ? q.OrderBy(n => n.Id)
                    : q.OrderByDescending(n => n.Id),
            };

        return (list, order);
    }
}