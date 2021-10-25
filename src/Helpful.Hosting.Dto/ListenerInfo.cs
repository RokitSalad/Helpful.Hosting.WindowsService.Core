using System.Security.Cryptography.X509Certificates;

namespace Helpful.Hosting.Dto
{
    /// <summary>
    /// Defines an HTTP binding.
    /// </summary>
    public class ListenerInfo
    {
        /// <summary>
        /// The IP address to listen on. Leave null to listen on all addresses.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Whether to use TLS.
        /// </summary>
        public bool UseTls { get; set; }

        /// <summary>
        /// The name of the store where the certificate is stored which should be used for this binding. Leave null if UseTls is false.
        /// </summary>
        public StoreName SslCertStoreName { get; set; }

        /// <summary>
        /// The subject of the certificate which should be used for this binding. Leave null if UseTls is false.
        /// </summary>
        public string SslCertSubject { get; set; }

        /// <summary>
        /// A flag to indicate whether to allow the located certificate to be used if it is invalid.
        /// </summary>
        public bool AllowInvalidCert { get; set; }
    }
}