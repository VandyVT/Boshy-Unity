using System.Collections;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class GoogleSheetsDownloader : MonoBehaviour
{


        public static List<List<string>> ParseCSV(string text)
        {
            text = CleanReturnInCsvTexts(text);

            var list = new List<List<string>>();
            var lines = Regex.Split(text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);

            bool jumpedFirst = false;

            foreach (var line in lines)
            {
                if (!jumpedFirst)
                {
                    jumpedFirst = true;
                    continue;
                }
                var values = Regex.Split(line, SPLIT_RE);

                var entry = new List<string>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    var value = values[j];
                    value = DecodeSpecialCharsFromCSV(value);
                    value = value.Replace("ÿ", "\"");
                    entry.Add(value);
                }
                list.Add(entry);
            }
            return list;
        }

public static string CleanReturnInCsvTexts(string text)
        {
            text = text.Replace("\"\"", "ÿ");

            if (text.IndexOf("\"") > -1)
            {
                string clean = "";
                bool insideQuote = false;
                for (int j = 0; j < text.Length; j++)
                {
                    if (!insideQuote && text[j] == '\"')
                    {
                        insideQuote = true;
                    }
                    else if (insideQuote && text[j] == '\"')
                    {
                        insideQuote = false;
                    }
                    else if (insideQuote)
                    {
                        if (text[j] == '\n')
                            clean += "<br>";
                        else if (text[j] == ',')
                            clean += "<c>";
                        else
                            clean += text[j];
                    }
                    else
                    {
                        clean += text[j];
                    }
                }
                text = clean;
            }
            //text = text.Replace("ÿ", "\"");
            return text;
        }

public static IEnumerator DownloadCSVCoroutine(string docId, Action<string> callback, bool saveAsset = false, string assetName = null, string sheetId = null)
        {
            string url =
                "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=csv";

            if (!string.IsNullOrEmpty(sheetId))
                url += "&gid=" + sheetId;

            WWWForm form = new WWWForm();
            WWW download = new WWW(url, form);

            yield return download;

            if (!string.IsNullOrEmpty(download.error))
            {
                Debug.Log("Error downloading: " + download.error);
            }
            else
            {
                //print("DL text is "+download.text);
                callback(download.text);
                if (saveAsset)
                {
                    if (!string.IsNullOrEmpty(assetName))
                        File.WriteAllText("Assets/Resources/" + assetName + ".csv", download.text);
                    else
                    {
                        throw new Exception("assetName is null");
                    }
                }
            }
            //print("Download operation is done. ID was " + sheetId);
        }

        //CSV reader from https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/

        public static readonly string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        public static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        public static readonly char[] TRIM_CHARS = { '\"' };

        public static List<List<string>> ReadCSV(string file)
        {
            var data = Resources.Load(file) as TextAsset;
            return ParseCSV(data.text);
        }

        public static string DecodeSpecialCharsFromCSV(string value)
        {
            value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("<br>", "\n").Replace("<c>", ",");
            return value;
        }
}
