// This section copied wholesale from karashiiro/TextToTalk
namespace wom_tts
{
    [Serializable]
    public class IpcMessage
    {

        /// <summary>
        /// The speaker name.
        /// </summary>
        public string? Speaker { get; init; }

        /// <summary>
        /// The message parameter - the spoken text for speech requests, and an empty string for cancellations.
        /// </summary>
        public string Payload { get; init; } = "";

        /// <summary>
        /// The message, with the player name replaced with a token.
        ///
        /// Full names are replaced with "{{FULL_NAME}}", first names are replaced with "{{FIRST_NAME}}", and last names
        /// are replaced with "{{LAST_NAME}}".
        /// </summary>
        public string PayloadTemplate { get; init; } = "";
    }
}