namespace SMART.ERP.Domain.Settings
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = null!;

        /// <summary>
        /// Client IDs adicionales aceptados como audiencia valida del id_token.
        /// Permite que otras aplicaciones (ej. una app nativa) usen el mismo endpoint.
        /// </summary>
        public List<string> AdditionalClientIds { get; set; } = new();
    }
}
