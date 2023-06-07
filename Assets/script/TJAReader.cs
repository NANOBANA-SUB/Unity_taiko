using System;
using System.IO;
using UnityEngine;

public class TJAReader : MonoBehaviour
{
    public string path = "path_to_tjafile.tja";
    public GameObject Don;
    public GameObject Katsu;
    public GameObject BigDon;
    public GameObject BigKatsu;

    private string title = "test"; //Default title
    private float BPM = 120f; //Default BPM
    private float MEASURE = 1f; //Default MEASURE

    void Start()
    {
        if (File.Exists(path))
        {
            ProcessFile();
        }
    }

    private void ProcessFile()
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            bool readingHeader = true;
            float measureStartPosition = 0f;  // This will hold the start x position of each measure

            while ((line = sr.ReadLine()) != null)
            {
                if (readingHeader)
                {
                    if (line.StartsWith("#START"))
                    {
                        readingHeader = false;
                        continue;
                    }
                    ProcessHeader(line);
                }
                else
                {
                    measureStartPosition = ProcessLine(line, measureStartPosition);
                }
            }
        }
    }

    private void ProcessHeader(string line)
    {
        if (line.StartsWith("TITLE:"))
        {
            title = line.Replace("TITLE:", "").Trim();
        }
        else if (line.StartsWith("BPM:"))
        {
            float.TryParse(line.Replace("BPM:", "").Trim(), out BPM);
        }
        else if (line.StartsWith("MEASURE:"))
        {
            string[] measureValues = line.Replace("MEASURE:", "").Trim().Split('/');
            if (measureValues.Length == 2)
            {
                MEASURE = float.Parse(measureValues[0]) / float.Parse(measureValues[1]);
            }
        }
    }

    private float ProcessLine(string line, float measureStartPosition)
    {
        if (line.StartsWith("#MEASURE"))
        {
            ProcessMeasureCommand(line);
        }
        else if (line.StartsWith("BPMCHANGE"))
        {
            ProcessBPMChangeCommand(line);
        }
        else
        {
            measureStartPosition = ProcessMeasures(line, measureStartPosition);
        }
        return measureStartPosition;
    }

    private void ProcessMeasureCommand(string line)
    {
        string[] commandValues = line.Replace("#MEASURE", "").Trim().Split('/');
        if (commandValues.Length == 2)
        {
            float numerator = float.Parse(commandValues[0]);
            float denominator = float.Parse(commandValues[1]);
            MEASURE = numerator / denominator;
        }
    }

    private void ProcessBPMChangeCommand(string line)
    {
        string[] commandValues = line.Replace("BPMCHANGE", "").Trim().Split('=');
        if (commandValues.Length == 2)
        {
            float newBPM;
            if (float.TryParse(commandValues[1], out newBPM))
            {
                BPM = newBPM;
            }
        }
    }

    private float ProcessMeasures(string line, float measureStartPosition)
    {
        // Trim the end of the line to remove any trailing commas or whitespace
        line = line.TrimEnd(',', ' ');

        string[] measures = line.Split(',');  // Split the note data into measures
        for (int i = 0; i < measures.Length; i++)
        {
            measureStartPosition = ProcessNotes(measures[i], measureStartPosition, i);
        }

        return measureStartPosition;
    }

    private float ProcessNotes(string measure, float measureStartPosition, int measureIndex)
    {
        char[] notesInMeasure = measure.ToCharArray();
        int totalNotes = notesInMeasure.Length;
        for (int j = 0; j < totalNotes; j++)
        {
            GameObject noteObject = CreateNoteObject(notesInMeasure[j]);
            if (noteObject != null)
            {
                NotePosition position = CalculateNotePosition(measureStartPosition, measureIndex, j, totalNotes);
                noteObject.transform.position = new Vector3(position.x, position.y, 0);
            }
        }
        measureStartPosition += MEASURE * 15;  // Update the start position for the next measure
        return measureStartPosition;
    }

    private GameObject CreateNoteObject(char noteChar)
    {
        GameObject noteObject = null;
        switch (noteChar)
        {
            case '1':
                noteObject = Instantiate(Don);
                break;
            case '2':
                noteObject = Instantiate(Katsu);
                break;
            case '3':
                noteObject = Instantiate(BigDon);
                break;
            case '4':
                noteObject = Instantiate(BigKatsu);
                break;
            // Add cases for '5', '6', '7' as needed
            case '0':
            default:
                break;
        }
        if (noteObject != null)
        {
            NoteMovement noteMovement = noteObject.AddComponent<NoteMovement>();
            noteMovement.Initialize(BPM);
        }
        return noteObject;
    }

    private NotePosition CalculateNotePosition(float measureStartPosition, int measureIndex, float noteIndex, float totalNotes)
    {
        float x = measureStartPosition + (noteIndex * (MEASURE * 15) / totalNotes) + (15 * measureIndex);
        float y = 0;
        return new NotePosition(x, y);
    }

    public class NotePosition
    {
        public float x;
        public float y;

        public NotePosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}