namespace Helpful.Hosting.Dto
{
    /// <summary>
    /// The Windows credentials to use for the service.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// The full Windows username and domain.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }
    }
}