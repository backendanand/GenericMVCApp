namespace GenericMVCApp.ServiceModels
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CookieSettings
    {
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public int ExpireTimeSpanMinutes { get; set; }
        public bool SlidingExpiration { get; set; }
    }
}
