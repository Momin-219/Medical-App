public static class DiagnosisLogic
{
    public static string GetDiagnosis(PatientData data)
    {
        // Retrieve the necessary values from the patient data
        data.Selections.TryGetValue("Azoospermia", out string azoospermia);
        data.Selections.TryGetValue("FSH", out string fsh);
        data.Selections.TryGetValue("Testes Size", out string testesSize);
        data.Selections.TryGetValue("Vas Deferens", out string vas);
        data.Selections.TryGetValue("Fructose", out string fructose);
        data.Selections.TryGetValue("Semen Volume", out string semenVolume);

        // --- Non-Obstructive Azoospermia (NOA) - Testicular Failure ---
        if (azoospermia == "Present" && fsh == "High" && testesSize == "< 4 ml")
        {
            return "Likely Non-Obstructive Azoospermia (Primary Testicular Failure). High FSH and small testes size suggest the testes are not producing sperm.";
        }

        // --- Obstructive Azoospermia (OA) - CBAVD ---
        if (azoospermia == "Present" && vas == "Not Palpable" && fructose == "Negative" && semenVolume == "< 1 ml")
        {
            return "Likely Obstructive Azoospermia due to Congenital Bilateral Absence of the Vas Deferens (CBAVD). Absence of vas, low volume, and no fructose are classic indicators.";
        }

        // --- Obstructive Azoospermia (OA) - Ejaculatory Duct Obstruction ---
        if (azoospermia == "Present" && vas == "Palpable" && fructose == "Negative" && semenVolume == "< 1 ml")
        {
            return "Likely Obstructive Azoospermia due to Ejaculatory Duct Obstruction (EDO). Normal vas but low volume and no fructose points to a blockage further down the tract.";
        }

        // --- Hypogonadotropic Hypogonadism ---
        if (fsh == "Low" && data.Selections.TryGetValue("LH", out string lh) && lh == "Low" && data.Selections.TryGetValue("Testosterone", out string testosterone) && testosterone == "Low")
        {
            return "Suggestive of Hypogonadotropic Hypogonadism. The pituitary gland is not signaling the testes correctly. Further investigation of the pituitary (e.g., MRI) is recommended.";
        }

        // Default diagnosis if no specific rule is met
        return "Diagnosis not conclusive based on the provided data. Further investigation may be required.";
    }
}