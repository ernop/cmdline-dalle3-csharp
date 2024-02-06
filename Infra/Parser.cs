using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dalle3
{
	static class Parser
	{
		public static IEnumerable<IPromptSection> ParseInput(string input)
		{
			var parts = new List<IPromptSection>();
			var regex = new Regex(@"\{[^\}]*\}|\[[^\]]*\]");

			int lastIndex = 0;
			var matches = regex.Matches(input);
            foreach (Match match in matches)
			{
				var textBeforeMatch = input.Substring(lastIndex, match.Index-lastIndex).Trim();
				
				if (!string.IsNullOrEmpty(textBeforeMatch))
				{
                    parts.Add(new TextPromptSection(textBeforeMatch));
				}

				// Add the matched text
				var delimiter = match.Value[0];
				var v = match.Value.Trim(new char[] { '{', '}', '[', ']', });
				if (delimiter == '{')
				{
					var a = new PermutationPromptSection(v);
                    parts.Add(a);
				}
                else if (delimiter == '[')
				{
					var a = new PowerSetPromptSection(v);
                    parts.Add(a);
                }

				lastIndex = match.Index + match.Length;
			}

			var remainingText = input.Substring(lastIndex).Trim();
			if (!string.IsNullOrEmpty(remainingText))
			{
                parts.Add(new TextPromptSection(remainingText));
            }

			return parts;
		}
	}
}