using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using XAP.Interface;
using Match = System.Text.RegularExpressions.Match;

namespace Xap.Engine
{
    public static class TokenParser
    {
        private static readonly Regex tokenPattern = new Regex(@"(\[\[.*\]\])", RegexOptions.Compiled);
        private static readonly Regex bracketsPattern = new Regex(@"\[\[|\]\]", RegexOptions.Compiled);

        private static readonly Regex multiLineTokenPattern = new Regex(@"\[{2}(\w+)=(.*?)\]{2}", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly char[] elementDelims = { ',', ';' };

        public static void ProcessTitleTokens(AlertInstance alert)
        {
            try
            {
                if (alert["Title"] == null || alert["Title"].Value == null)
                {
                    return;
                }

                string title = alert["Title"].Value;

                var tokensMatch = tokenPattern.Match(title);

                if (!tokensMatch.Success)
                {
                    return;
                }
                alert.AddTrace("Found tokens in the alert title, processing...");
                var tokens = ParseTokens(tokensMatch.Groups[1].Value);

                alert["Title"].Value = tokenPattern.Replace(title, string.Empty);

                foreach (var token in tokens)
                {
                    if (alert[token.Key] == null)
                    {
                        alert.AddTrace("Adding new property '{0}' with value '{1}", token.Key, token.Value);
                        alert.AddProperty(token.Key, token.Value);
                    }
                    else
                    {
                        alert.AddTrace("Replacing existing property '{0}' with value '{1}'. The previous value was '{2}",
                            token.Key, token.Value, alert[token.Key].Value);

                        alert[token.Key].Value = token.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                alert.AddTrace(TraceEventType.Warning, -999, "Unable to parse tokens: " + ex);
            }
        }

        public static AlertInstance ProcessMultilineTokens(string input)
        {
            var ai = new AlertInstance {XapId = Guid.NewGuid()};

            var matches = multiLineTokenPattern.Matches(input);

            foreach (var match in matches.Cast<Match>())
            {
                if (!match.Success || match.Groups.Count != 3)
                {
                    continue;
                }

                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                ai.AddProperty(key, value);
            }

            return ai;
        }

        private static Dictionary<string, string> ParseTokens(string value)
        {
            var values = new Dictionary<string, string>();
            var noBrackets = bracketsPattern.Replace(value, string.Empty);

            var tokens = noBrackets.Split(elementDelims, StringSplitOptions.RemoveEmptyEntries)
                .Select(token => token.Trim())
                .Select(trimmedToken => trimmedToken.Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                .Where(token => token.Length == 2)
                .Where(token => !values.ContainsKey(token[0].Trim()));

            foreach (var token in tokens)
            {
                values.Add(token[0].Trim(), token[1].Trim());
            }

            return values;
        }
    }
}
