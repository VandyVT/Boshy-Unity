#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GoogleSheetsJSONUpdater : MonoBehaviour
{

    public string link = "1aeIhzvW4l0OjwHAk9S6yif47Lr_xGByZumZVluPVsgg";

    public string[] langs;

    [System.Serializable]
    public class sheet {

        public string referenceName;
        public string sheetID;
        public bool dataLoaded;
        public string rawContent;
        public List<List<string>> acquiredContents;

        public int namesColumn = 1;
        public List<string> namesColumnContent;

        public void OutputCallback (string output)  {

            acquiredContents = GoogleSheetsDownloader.ParseCSV(output);

             namesColumnContent = new List<string>();

            foreach(List<string> demo in acquiredContents) {
                namesColumnContent.Add (demo[namesColumn]);
            }
            dataLoaded = true;
        }

    }
    public sheet[] m_sheets;

    public bool export;

     void OnEnable()
    {
        foreach(sheet targetSheet in m_sheets)  { 
            
            targetSheet.dataLoaded = false;
            StartCoroutine(GoogleSheetsDownloader.DownloadCSVCoroutine(link, targetSheet.OutputCallback, false, null, targetSheet.sheetID));
            
        }
    }

    void Update ()  {

        
        if(export)  {

            exportSheets();
            export = false;
        }

    }

    public void exportSheets ()  {

        string resourcesPath = (Application.dataPath + "/UI/Newgame+ LD/Resources/Text");

        if(Directory.Exists(resourcesPath)) print("FOUND: "+resourcesPath);
        else print("MISSING: "+resourcesPath);

        List<List<string>> langSheets = new List<List<string>>();

        //Iterate through spreadsheet pages
        foreach(sheet targetSheet in m_sheets)  { 

            if(!targetSheet.dataLoaded) {

                Debug.LogError("TEXT EXPORT FAILURE: Not all data has been loaded yet!");
                return;
            }

            List<string> sheetJsonConversions = new List<string>();

            //Gather all the languages
            for(int i = 0; i < langs.Length; i++) {

                //print("Compiling sheet " + targetSheet.referenceName + " " + langs[i]);

                sheetJsonConversions.Add("");

                int targetColumn = targetSheet.namesColumn + i + 1;

                sheetJsonConversions[i] += "{\n";

                //Gather all the boxes
                for(int j = 0; j < targetSheet.namesColumnContent.Count; j++)   {

                    sheetJsonConversions[i] += (targetSheet.namesColumnContent[j] + targetSheet.acquiredContents[j][targetColumn] + "\n");

                    //print("Iteration through box " + j + "   result was " +(targetSheet.namesColumnContent[j] + targetSheet.acquiredContents[j][targetColumn]));

                }
                sheetJsonConversions[i] += "\n}";

                print(targetSheet.referenceName + "  " + langs[i] + "\n\n" + sheetJsonConversions[i]);
            }

            langSheets.Add(sheetJsonConversions);

            
        }

        //print(langSheets[0][1]);
        //Now that we're done logging everything, now's time to export everything! Format: [Sheet][Language]
        for(int i = 0; i < langSheets.Count; i++)   {

            for(int j = 0; j < langs.Length; j++)   {

                string directoryPath = (resourcesPath + "/" + langs[j]);

                if(!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                File.WriteAllText(directoryPath+"/txt_"+m_sheets[i].referenceName+"."+langs[j]+".txt", langSheets[i][j]);
                Debug.LogWarning("FILE CREATED: " + "txt_"+m_sheets[i].referenceName+"."+langs[j]+".txt\n\n" + langSheets[i][j]);
            }

        }

    }

  
}
#endif