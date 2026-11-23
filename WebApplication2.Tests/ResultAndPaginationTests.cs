using WebApplication2.Results;

namespace WebApplication2.Tests;

public class ResultAndPaginationTests
{
    [Fact]
    public void ResultSuccess_IsSuccessfulAndHasNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void ResultFailure_PreservesError()
    {
        const string error = "An operation failed";

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericResultSuccess_ReturnsValue()
    {
        var result = GenericResult<string>.Success("saved");

        Assert.True(result.IsSuccess);
        Assert.Equal("saved", result.Value);
        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void GenericResultFailure_PreservesErrorAndUsesDefaultValue()
    {
        const string error = "Record not found";

        var result = GenericResult<int>.Failure(error);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void PaginatedList_DefaultsToFirstPageAndTenItems()
    {
        var result = new PaginatedList<string>();

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public void PaginatedList_PageSizeAboveMaximum_IsCappedAtTwenty()
    {
        var result = new PaginatedList<string> { PageSize = 25 };

        Assert.Equal(20, result.PageSize);
    }

    [Fact]
    public void PaginatedList_PageSizeWithinMaximum_IsPreserved()
    {
        var result = new PaginatedList<string> { PageSize = 15 };

        Assert.Equal(15, result.PageSize);
    }

    [Fact]
    public void PaginatedList_StoresDataAndPagingMetadata()
    {
        var data = new[] { "first", "second" };
        var result = new PaginatedList<string>
        {
            PageNumber = 2,
            PageSize = 2,
            TotalRecords = 5,
            Totalpage = 3,
            Data = data
        };

        Assert.Equal(data, result.Data);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(3, result.Totalpage);
    }
}
