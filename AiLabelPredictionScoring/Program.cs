using System.Text.Json;

// Simple simulation to illustrate and understand Precision, Recall and F1 Score

// Top-level statements
var fileName = "trainingData.json";
var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

// Deserialize the JSON into a list of TrainingData objects
var trainingData = LoadTrainingData(filePath);

// Ensure data was loaded successfully
if (trainingData == null || trainingData.Count == 0)
{
    Console.WriteLine("Failed to load training data or training data is empty.");
    return;
}

// Example of processing the deserialized data
var prediction = new Prediction(trainingData, "taco");
var scorer = new Scorer(prediction);
scorer.Score();
scorer.DisplayConfusionMatrix();

// Display the results
Console.WriteLine($"\nPrecision: {scorer.Precision:F2}");
Console.WriteLine($"Recall: {scorer.Recall:F2}");
Console.WriteLine($"F1 Score: {scorer.F1Score:F2}");

// Output each data item and its type (TP, FP, FN, TN)
Console.WriteLine("\nDetailed Results:");
foreach (var dataItem in trainingData)
{
    string resultType = Prediction.Predict(dataItem, prediction.TargetLabel);
    Console.WriteLine($"Text: {dataItem.ItemDescription}");
    Console.WriteLine($"Actual Label: {dataItem.ActualLabel}");
    Console.WriteLine($"Predicted Label: {dataItem.PredictedLabel}");
    Console.WriteLine($"Type: {resultType}");
    Console.WriteLine();
}

Console.ReadLine();

// Helper method to deserialize JSON from a file
static List<TrainingData> LoadTrainingData(string filePath)
{
    string jsonString = File.ReadAllText(filePath);
    var jsonData = JsonSerializer.Deserialize<List<TrainingData>>(jsonString);
    return jsonData ?? new List<TrainingData>();
}

// Using record classes with init properties for deserialization
public record TrainingData
{
    public required string ItemDescription { get; init; }
    public required string ActualLabel { get; init; }
    public required string PredictedLabel { get; init; }
}

// Prediction class, using records with calculated properties
public record Prediction(IList<TrainingData> Data, string TargetLabel)
{
    public int TruePositives => Data.Count(kv => kv.ActualLabel == TargetLabel && kv.PredictedLabel == TargetLabel);
    public int FalsePositives => Data.Count(kv => kv.ActualLabel != TargetLabel && kv.PredictedLabel == TargetLabel);
    public int FalseNegatives => Data.Count(kv => kv.ActualLabel == TargetLabel && kv.PredictedLabel != TargetLabel);
    public int TrueNegatives => Data.Count(kv => kv.ActualLabel != TargetLabel && kv.PredictedLabel != TargetLabel);

    public static string Predict(TrainingData dataItem, string targetLabel) =>
    (dataItem.ActualLabel, dataItem.PredictedLabel) switch
    {
        var (actual, predicted) when actual == targetLabel && predicted == targetLabel => "True Positive (TP)",
        var (actual, predicted) when actual != targetLabel && predicted == targetLabel => "False Positive (FP)",
        var (actual, predicted) when actual == targetLabel && predicted != targetLabel => "False Negative (FN)",
        var (actual, predicted) when actual != targetLabel && predicted != targetLabel => "True Negative (TN)",
        _ => "Unknown"
    };
}

// Scorer class with methods to calculate metrics
public class Scorer
{
    public double Precision { get; private set; }
    public double Recall { get; private set; }
    public double F1Score { get; private set; }

    private readonly Prediction _prediction;

    public Scorer(Prediction prediction)
    {
        _prediction = prediction;
    }

    public void Score()
    {
        // Precision: Measures the accuracy of positive predictions (i.e., the percentage of correctly identified target objects 
        // among all items predicted to be target objects). Precision = TP / (TP + FP)
        Precision = (double)_prediction.TruePositives / (_prediction.TruePositives + _prediction.FalsePositives);

        // Recall (or Sensitivity): Measures the ability to identify all positive examples (i.e., the percentage of target objects
        // correctly identified among all actual target objects). Recall = TP / (TP + FN)
        Recall = (double)_prediction.TruePositives / (_prediction.TruePositives + _prediction.FalseNegatives);

        // F1 Score: The harmonic mean of Precision and Recall, providing a single metric that balances both concerns.
        // It is useful when there is an uneven class distribution, and we want a balance between precision and recall.
        // F1 Score = 2 * (Precision * Recall) / (Precision + Recall)
        F1Score = 2 * (Precision * Recall) / (Precision + Recall);
    }


    public void DisplayConfusionMatrix()
    {
        Console.WriteLine($"\nConfusion Matrix for target label: '{_prediction.TargetLabel}'");
        Console.WriteLine($"                   Predicted: {_prediction.TargetLabel}  Predicted: Not {_prediction.TargetLabel}");
        Console.WriteLine($"Actual: {_prediction.TargetLabel}      TP: {_prediction.TruePositives}          FP: {_prediction.FalsePositives}");
        Console.WriteLine($"Actual: Not {_prediction.TargetLabel}  FN: {_prediction.FalseNegatives}          TN: {_prediction.TrueNegatives}");
    }
}
