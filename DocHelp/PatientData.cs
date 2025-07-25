using System.Collections.Generic;

public class PatientData
{
    public string PatientName { get; set; }
    public string PatientId { get; set; }
    public string Age { get; set; }
    public string Symptoms { get; set; }
    public Dictionary<string, string> Selections { get; set; }

    // --- NEW PROPERTY ---
    public string DiagnosisResult { get; set; }

    public PatientData()
    {
        Selections = new Dictionary<string, string>();
    }
}