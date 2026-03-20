namespace Agenda.Core.QueryFilters;

public class UsersQueryFilter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchCriteria { get; set; }
}