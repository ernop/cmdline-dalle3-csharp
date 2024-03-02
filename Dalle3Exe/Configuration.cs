using OpenAI_API.Images;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;

namespace Dalle3
{

    /// <summary>
    /// hey user, for now, you need a c# compiler anyway, so you'd better use this :
    /// </summary>
    public static class Configuration
    {
        public static string LogPath = "../../logs/log.txt";
        public static string ApiKey { get; set; } = System.IO.File.ReadAllText("../../apikey.txt");
        public static string OrgId { get; set; } = System.IO.File.ReadAllText("../../organization.txt");

        /// <summary>
        /// for some dumb reason, this has to exist at this target location
        /// </summary>
        public static string FakeGraphicsPngAnyPngWorks { get; set; } = "../../image.png";

        public static Random Random = new Random();

        /// <summary>
        /// Configure your tier yourself! visit this page: https://platform.openai.com/docs/guides/rate-limits/usage-tiers while logged in
        /// and figure out what tier you are. This tier (based on your total historical payments to OpenAI) controls your rate limits.
        /// This data is current as of 2/2024 but will probably change.
        /// TODO obviously, put this into a config file so its easier to manage.
        /// ALSO you can make this lower. I have never tried 5, the speed is scary.
        /// </summary>
        public static int MyOpenAiTier { get; set; } = 4;
    }
}
