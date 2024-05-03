namespace Dalle3
{
    /// <summary>
    /// When an IPromptSection return a string to a prompt maker,
    /// it can be useful to also let the generator tell the thing
    /// whether that resulted in some kind of content filtering
    /// or like that. then perhaps someday it would 
    /// </summary>
    public enum TextChoiceResultEnum
    {
        Okay = 1,
        PromptRejected = 2,
        ImageDescriptionsGeneratedBad = 3,
        RequestBlocked = 4,
        RateLimit=5,
        RateLimitRepeatedlyExceeded = 6,
        UnknownError = 7,
        TooLong = 8,
        BillingLimit = 9,
        TaskCancelled = 10,
    }
}
