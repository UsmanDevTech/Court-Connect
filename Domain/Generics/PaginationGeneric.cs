using Domain.Contracts;

namespace Domain.Generics;

public class PaginationRequestBase
{
    public int? pageNumber { get; set; }
    public int? pageSize { get; set; }
    public PaginationRequestBase()
    {
        pageNumber = 1;
        pageSize = 10;
    }
    public PaginationRequestBase(int? pageNumber, int? pageSize)
    {
        this.pageNumber = pageNumber < 1 || pageNumber == null ? 1 : pageNumber;
        this.pageSize = pageSize > 10 || pageSize == null ? 10 : pageSize;
    }
}
public class PaginationRequestBaseGeneric<T> : PaginationRequestBase
{
    public T? data { get; set; }
    public PaginationRequestBaseGeneric() : base()
    {
    }
    public PaginationRequestBaseGeneric(int? pageNumber, int? pageSize, T data) : base(pageNumber, pageSize)
    {
        this.data = data;
    }
}

public class PaginationResponseBase<T>
{
    public T? data { get; set; }
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public int totalPages { get; set; }
    public int totalRecords { get; set; }
    public PaginationResponseBase(T data, int pageNumber, int pageSize, int totalPages, int totalRecords)
    {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.totalPages = totalPages;
        this.totalRecords = totalRecords;
        this.data = data;
    }
}
public class PaginationResponseBaseWithActionRequest<T>
{
    public T? data { get; set; }
    public RequestBaseContract request { get; set; }
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public int totalPages { get; set; }
    public int totalRecords { get; set; }
    public PaginationResponseBaseWithActionRequest(T data, RequestBaseContract request, int pageNumber, int pageSize, int totalPages, int totalRecords)
    {
        this.pageNumber = pageNumber;
        this.pageSize = pageSize;
        this.totalPages = totalPages;
        this.totalRecords = totalRecords;
        this.data = data;
        this.request = request;
    }
}
