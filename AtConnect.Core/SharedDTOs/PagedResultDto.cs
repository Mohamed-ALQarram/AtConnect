namespace AtConnect.Core.SharedDTOs
{
    public class PagedResultDto<T>
    {
        public PaginationMetadataDto Metadata { get; set; }
        public List<T> Items { get; set; }
        

        public PagedResultDto(List<T> items, int count, int pageNumber, int pageSize)
        {
            Metadata = new PaginationMetadataDto(count, pageNumber, pageSize);
            Items = items;
        }
    }
}
