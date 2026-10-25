namespace FinBooKeApp.Core.Services.Categories;

public enum CategoryServiceResultCode
{
    OK = 1,
    CATEGORY_DOES_NOT_EXIST = 2,
    CHILD_CATEGORY_DOES_NOT_EXIST = 3,
    PARENT_CATEGORY_DOES_NOT_EXIST = 4,
    CATEGORY_ALREADY_EXIST = 5,
    INVALID_CATEGORY_LIMIT = 6,
    INVALID_CATEGORY_DEPENDENCY = 7,
}
