namespace AuthGEO
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JWTRole : Attribute
    {
        public string Role { get; set; }
        public JWTRole(string role)
        {
            this.Role = role;
        }
    }
}
