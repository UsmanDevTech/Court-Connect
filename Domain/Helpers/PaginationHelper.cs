using Domain.Generics;

namespace Domain.Helpers;

public static class PaginationHelper
{
    public static PaginationResponseBase<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationRequestBase validFilter, int totalRecords)
    {
        var totalPages = ((double)totalRecords / (double) validFilter.pageSize.Value);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        var respose = new PaginationResponseBase<List<T>>(pagedData, validFilter.pageNumber ?? 0, validFilter.pageSize ?? 0, roundedTotalPages, totalRecords);
        return respose;
    }
    public static DatatableResponse<List<T>> CreateDatatableReponse<T>(List<T> pagedData, DataTablePaginationFilter validFilter, int totalRecords)
    {
        var respose = new DatatableResponse<List<T>>(pagedData, validFilter.pageNumber.Value, validFilter.pageSize.Value);
        var totalPages = ((double)totalRecords / (double)validFilter.pageSize);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        respose.NextPage = validFilter.pageNumber >= 1 && validFilter.pageNumber < roundedTotalPages
            ? validFilter.pageNumber.Value + 1 : null;

        respose.PreviousPage = validFilter.pageNumber - 1 >= 1 && validFilter.pageNumber <= roundedTotalPages
            ? validFilter.pageNumber.Value - 1 : null;
        respose.FirstPage = 1;
        respose.LastPage = roundedTotalPages;
        respose.totalPages = roundedTotalPages;
        respose.totalRecords = totalRecords;
        return respose;
    }
}
