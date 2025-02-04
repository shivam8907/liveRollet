using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

public class PrintingTool : MonoBehaviour
{
    private static PrintingTool _instance;

    public static PrintingTool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("PrintingTool");
                _instance = go.AddComponent<PrintingTool>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private string printerName = "EPSON TM-T82-42C Receipt";
    private Thread m_thread;

    private void OnDisable() 
    {
        if (m_thread != null)
            m_thread.Abort();
    }

    private void OnDestroy()
    {
        if (m_thread != null)
            m_thread.Abort();
    }

    private void OnApplicationQuit()
    {
        if (m_thread != null)
            m_thread.Abort();
    }

    public static void CmdPrintThreaded(string printerName, string _filePath)
    {
        string fullCommand = "rundll32 C:\\WINDOWS\\system32\\shimgvw.dll,ImageView_PrintTo " + "\"" + _filePath + "\"" + " " + "\"" + printerName + "\"";
        Thread m_thread = new Thread(delegate () { CmdPrint(fullCommand); });
        m_thread.IsBackground = true;
        m_thread.Start();
    }

    static void CmdPrint(string _command)
    {
        Process myProcess = new Process();
        myProcess.StartInfo.CreateNoWindow = true;
        myProcess.StartInfo.UseShellExecute = false;
        myProcess.StartInfo.FileName = "cmd.exe";
        myProcess.StartInfo.Arguments = "/c " + _command;
        myProcess.EnableRaisingEvents = true;
        myProcess.Start();
        myProcess.WaitForExit();
    }

    int GetPrintJobCount()
    {
        try
        {
            Process myProcess = new Process();
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = "/c " + "cscript C:\\Windows\\System32\\Printing_Admin_Scripts\\en-US\\prnjobs.vbs -l";
            myProcess.StartInfo.RedirectStandardOutput = true; // capture the Standard Output stream
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();

            StringBuilder sb = new StringBuilder();
            while (!myProcess.HasExited)
                sb.Append(myProcess.StandardOutput.ReadToEnd());

            myProcess.WaitForExit();

            string resultString = sb.ToString();

            string[] resultStringArray = resultString.Split(' ').ToArray();

            string jobCountString = resultStringArray[resultStringArray.Length - 1];
            UnityEngine.Debug.Log("Print job counts: " + jobCountString);
            return (int.Parse(jobCountString));
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            return -1;
        }
    }
}