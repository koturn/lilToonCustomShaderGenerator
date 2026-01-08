using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator
{
    /// <summary>
    /// Entry point class
    /// </summary>
    public class TemplateEngine
    {
        public string NewLine { get; set; }

        private readonly Dictionary<string, string> _replaceDef = new Dictionary<string, string>();

        public TemplateEngine()
            : this(null, Environment.NewLine)
        {
        }

        public TemplateEngine(Dictionary<string, string> replaceDef)
            : this(replaceDef, Environment.NewLine)
        {
        }

        public TemplateEngine(Dictionary<string, string> replaceDef, string newLine)
        {
            if (replaceDef != null)
            {
                var d = _replaceDef;
                foreach (var kv in replaceDef)
                {
                    d[kv.Key] = kv.Value;
                }
            }
            NewLine = newLine;
        }

        public void AddTag(string name, string val)
        {
            _replaceDef.Add(name, val);
        }

        public string GetTag(string name)
        {
            return _replaceDef[name];
        }

        public void ExpandTemplate(string templatePath, string targetPath)
        {
            using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var targetStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                ExpandTemplate(templateStream, targetStream);
            }
        }

        public void ExpandTemplate(FileStream templateStream, FileStream targetStream)
        {
            using (var reader = new StreamReader(templateStream))
            using (var writer = new StreamWriter(targetStream)
            {
                NewLine = NewLine
            })
            {
                ExpandTemplate(reader, writer);
            }
        }

        public void ExpandTemplate(StreamReader reader, StreamWriter writer)
        {
            var replaceDef = _replaceDef;
            var shouldEmit = true;
            var shouldEmiStack = new Stack<bool>();
            var lineCount = 0;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineCount++;

                if (line.StartsWith("!!"))
                {
                    if (Regex.IsMatch(line, @"^!!\s*endif!!\s*$"))
                    {
                        shouldEmit = shouldEmiStack.Pop();
                        continue;  // Not emit !!endif!! line
                    }
                    else if (Regex.IsMatch(line, @"^!!\s*else!!\s*$"))
                    {
                        if (shouldEmiStack.Count == 0)
                        {
                            throw new InvalidOperationException("\"else\" is detected out of if context at line " + lineCount + ".");
                        }
                        shouldEmit = !shouldEmit;
                        continue;  // Not emit !!else!! line
                    }
                    else
                    {
                        var m = Regex.Match(line, @"^!!\s*(el)?if(not)?empty:\s*(\w+)\s*!!\s*$");
                        if (m.Success)
                        {
                            var g = m.Groups;
                            if (string.IsNullOrEmpty(g[1].Value))
                            {
                                shouldEmiStack.Push(shouldEmit);
                            }

                            // if
                            // var hasTagValue = string.IsNullOrEmpty(replaceDef.GetValueOrDefault(g[3].Value));
                            var tag = g[3].Value;
                            var hasTagValue = replaceDef.ContainsKey(tag) && !string.IsNullOrEmpty(replaceDef[tag]);
                            if (string.IsNullOrEmpty(g[2].Value))
                            {
                                // ifempty
                                shouldEmit = !hasTagValue;
                            }
                            else
                            {
                                // ifnotempty
                                shouldEmit = hasTagValue;
                            }
                            continue;  // Not emit !!ifempty!!, !!ifnotempty!!, !!elifempty!!, !!elifnotempty!! line
                        }
                    }
                }
                if (!shouldEmit)
                {
                    continue;
                }

                var replacedLine = Replace(line);
                if (replacedLine != null)
                {
                    writer.Write(replacedLine);
                    writer.Write(NewLine);
                }
            }

            if (shouldEmiStack.Count > 0)
            {
                throw new InvalidOperationException("Non closed if");
            }
        }

        private Regex _regex = new Regex(@"%%(\w+)\s*(?::\s*(.+))?%%");

        public string Replace(string text)
        {
            if (!text.Contains("%%"))
            {
                return text;
            }

            var replaceDef = _replaceDef;
            var sb = new StringBuilder();
            var m = _regex.Match(text);
            var parsedIndex = 0;
            while (m.Success)
            {
                var g = m.Groups;

                if (g[0].Index > parsedIndex)
                {
                    sb.Append(text, parsedIndex, g[0].Index - parsedIndex);
                    parsedIndex = g[0].Index;
                }

                var tag = g[1].Value;
                if (replaceDef.ContainsKey(tag))
                {
                    var content = replaceDef[tag];
                    var indentString = string.Empty;
                    var isKeepIndent = false;
                    var optionPart = m.Groups[2].Value;
                    foreach (var option in optionPart.Split(':'))
                    {
                        if (option.StartsWith("spaceindent="))
                        {
                            indentString = new string(' ', int.Parse(option.Substring(12)));
                            isKeepIndent = false;
                        }
                        else if (option.StartsWith("tabindent="))
                        {
                            indentString = new string('\t', int.Parse(option.Substring(10)));
                            isKeepIndent = false;
                        }
                        else if (option == "keepindent")
                        {
                            var m2 = Regex.Match(text, "^\\s+");
                            if (m2.Success)
                            {
                                indentString = m2.Groups[0].Value;
                            }
                            else
                            {
                                indentString = string.Empty;
                            }
                            isKeepIndent = true;
                        }
                        else if (option == "skipempty")
                        {
                            if (content.Length == 0)
                            {
                                return null;
                            }
                        }
                    }

                    using (var ssr = new StringReader(content))
                    {
                        int writeLineCount = 0;
                        string contentLine;
                        while ((contentLine = ssr.ReadLine()) != null)
                        {
                            if (writeLineCount > 0)
                            {
                                sb.Append(NewLine);
                                sb.Append(indentString);
                            }
                            else if (!isKeepIndent)
                            {
                                sb.Append(indentString);
                            }
                            sb.Append(contentLine);
                            writeLineCount++;
                        }
                    }
                }
                else
                {
                    Debug.LogWarningFormat("tag \"{0}\" is not defined", tag);
                }

                parsedIndex += g[0].Length;
                m = _regex.Match(text, parsedIndex);
            }

            sb.Append(text.Substring(parsedIndex));

            return sb.ToString();
        }
    }
}
