namespace Chirp.Web.Pages;

public class PaginationModel
{
    
    private readonly PublicModel _parentModel;

    public PaginationModel(PublicModel parentModel)
    {
        _parentModel = parentModel;
    }
    
    public int CurrentPage => _parentModel._currentPage;
    public bool NextPageHasCheeps;
    public int ItemsPerPage { get; set; } = 32; // Default page size
    
    private
    
    
}