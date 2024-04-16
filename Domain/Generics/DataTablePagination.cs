
using Domain.Contracts;

namespace Domain.Generics;
public class DataTablePaginationBase<T>
{
    public T data { get; set; }
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public int totalPages { get; set; }
    public int totalRecords { get; set; }
}
public class DatatableResponse<T> : DataTablePaginationBase<T>
{

    public int? FirstPage { get; set; }
    public int? LastPage { get; set; }
    public int? NextPage { get; set; }
    public int? PreviousPage { get; set; }
    public DatatableResponse(T data, int pageNumber, int pageSize)
    {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.data = data;
    }
}
public class DataTablePaginationFilter : PaginationRequestBase
{
    public int? status { get; set; }
    public string? sortColumn { get; set; }
    public string? sortColumnDirection { get; set; }
    public string? searchValue { get; set; }
    public DataTablePaginationFilter(int? pageNumber, int? pageSize, int? status, string? sortColumn, string? sortColumnDirection, string? searchValue)
    {
        this.pageNumber = pageNumber < 1 || pageNumber == null ? 0 : pageNumber;
        this.pageSize = pageSize > 250 || pageSize == null ? 250 : pageSize;
        this.status = status;
        this.sortColumn = sortColumn;
        this.sortColumnDirection = sortColumnDirection;
        this.searchValue = searchValue;
    }
}
public class GenericPreQueryListModel<T>
{
    public string? createdBy { get; set; }
    public string? updatedBy { get; set; }
    public DateTime? createdAt { get; set; }
    public DateTime? lastUpdatedAt { get; set; }
    public T data { get; set; }
}
public class GenericPostQueryListModel<T>
{
    public LeagueRankingUserContract createdBy { get; set; }
    public string? updatedBy { get; set; }
    public string? createdAt { get; set; }
    public string? lastUpdatedAt { get; set; }
    public T data { get; set; }
}

