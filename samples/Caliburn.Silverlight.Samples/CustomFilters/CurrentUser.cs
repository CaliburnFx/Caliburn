namespace CustomFilters
{
    public static class CurrentUser
    {
        public static bool IsInRole(string role)
        {
            return role.Equals("User");
        }
    }
}