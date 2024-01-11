using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSheetsDebugger : MonoBehaviour
{
    public TMP_Text outputText;
    public string link = "12JbPu89t0XOUCiQNreuUbnj9hnPRM0mDzf3qBb5tpac";
    public string sheetId = "characommon";
    public int refColumn = 1;
    [TextArea(3, 50)]
    public string demonstration;


    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(GoogleSheetsDownloader.DownloadCSVCoroutine(link, OutputCallback, false, null, sheetId));
    }

    void OutputCallback(string output)
    {

        List<List<string>> twoDee = GoogleSheetsDownloader.ParseCSV(output);

        demonstration = "";

        foreach (List<string> demo in twoDee)
        {
            demonstration += (demo[refColumn]);
        }

        outputText.text = demonstration;
    }


}