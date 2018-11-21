namespace Employees.Models
{
    public class PagingParameterModel
    {
        const int maxPageSize = 20;
        const string defaultSortOrder = "asc";

        public string _sortOrder { get; set; } = "asc";

        public string columnName { get; set; } = "Name";

        public string querySearchName { get; set; } = "Name";

        public int pageNumber { get; set; } = 1;

        public int _pageSize { get; set; } = 10;

        public int pageSize
        {

            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public string sortOrder
        {

            get { return _sortOrder; }
            set
            {
                _sortOrder = (value != "desc") ? defaultSortOrder : value;
            }
        }

        public string querySearch { get; set; }
    }
}