namespace WorkoutBuddy.Util;

public class Paginated<T>
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public bool LastPage { get; set; }
    public List<T> Items { get; set; } = new List<T>();

    public Paginated(int totalPages, int currentPage, int totalItems, bool lastPage, List<T> items)
    {
        TotalPages = totalPages;
        CurrentPage = currentPage;
        TotalItems = totalItems;
        LastPage = lastPage;
        Items = items;
    }
}