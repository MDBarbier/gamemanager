namespace gamemanager.Models
{
    public class StoreDataDlc
    {
        public long Id { get; set; }
        public string StoreUrl { get; set; }
        public string StoreName { get; set; }
        public int AppId { get; set; }
        public int ParentId { get; set; }
    }
}
