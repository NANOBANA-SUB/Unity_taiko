using System.Collections;
using System.IO;
using UnityEngine;

public class TJAPlayer : MonoBehaviour
{
    public string path = "path_to_tjafile.tja";
    public GameObject Don;
    public GameObject Katsu;
    public GameObject BigDon;
    public GameObject BigKatsu;

    private string title = "test"; //Default title
    private float baseBPM = 120f; //Base BPM
    private float MEASURE = 1f; //Default MEASURE

    private void ReadingHeader(string line)
    {
        if (line.StartsWith("TITLE:"))
        {
            title = line.Replace("TITLE:", "").Trim();
        }
        else if (line.StartsWith("BPM:"))
        {
            float.TryParse(line.Replace("BPM:", "").Trim(), out baseBPM);
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

    public NotePosition CalculateNotePosition(float measureStartPosition, float noteIndex, float totalNotes)
    {
        float x = measureStartPosition + (noteIndex * (MEASURE * 15) / totalNotes);
        float y = 0;

        return new NotePosition(x, y);
    }

    void Start()
    {
        // Check if the file exists
        if (File.Exists(path))
        {
            // Open the file
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                bool readingHeader = true;
                float measureStartPosition = 0f;

                while ((line = sr.ReadLine()) != null)
                {
                    if (readingHeader)
                    {
                        if (line.StartsWith("#START"))
                        {
                            readingHeader = false;
                            continue;
                        }
                        ReadingHeader(line);
                    }
                    else
                    {
                        if (line.StartsWith("#END"))
                        {
                            break;
                        }

                        if (line.StartsWith("#MEASURE"))
                        {
                            // Process the #MEASURE command
                            string[] commandValues = line.Replace("#MEASURE", "").Trim().Split('/');
                            if (commandValues.Length == 2)
                            {
                                float numerator = float.Parse(commandValues[0]);
                                float denominator = float.Parse(commandValues[1]);
                                MEASURE = numerator / denominator;
                            }
                            continue;
                        }
                        else if (line.StartsWith("BPMCHANGE"))
                        {
                            // Process the BPMCHANGE command
                            string[] commandValues = line.Replace("BPMCHANGE", "").Trim().Split('=');
                            if (commandValues.Length == 2)
                            {
                                float newBPM;
                                if (float.TryParse(commandValues[1], out newBPM))
                                {
                                    baseBPM = newBPM;
                                }
                            }
                            continue;
                        }

                        string[] measures = line.Split(',');  // Split the note data into measures

                        for (int i = 0; i < measures.Length; i++)
                        {
                            string measure = measures[i];
                            char[] notesInMeasure = measure.ToCharArray();

                            int totalNotes = notesInMeasure.Length;

                            for (int j = 0; j < totalNotes; j++)
                            {
                                char noteChar = notesInMeasure[j];

                                GameObject noteObject;
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
                                    default:
                                        continue;
                                }

                                NotePosition position = CalculateNotePosition(measureStartPosition, j, totalNotes);
                                noteObject.transform.position = new Vector3(position.x, position.y, 0);
                            }

                            measureStartPosition += MEASURE * 15;
                        }
                    }
                }
            }
        }
    }
}
