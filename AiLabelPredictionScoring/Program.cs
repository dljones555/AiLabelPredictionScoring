using System.Text.Json;
using System.Text.Json.Serialization;

// Simple simulation to illustrate and understand Precision, Recall and Precision

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
    string resultType = DetermineResultType(dataItem, prediction.TargetLabel);
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
    var jsonData = JsonSerializer.Deserialize<TrainingDataContainer>(jsonString);
    return jsonData?.Data ?? new List<TrainingData>();
}

// Method to determine if the item is TP, FP, FN, or TN
static string DetermineResultType(TrainingData dataItem, string targetLabel) =>
    (dataItem.ActualLabel, dataItem.PredictedLabel) switch
    {
        var (actual, predicted) when actual == targetLabel && predicted == targetLabel => "True Positive (TP)",
        var (actual, predicted) when actual != targetLabel && predicted == targetLabel => "False Positive (FP)",
        var (actual, predicted) when actual == targetLabel && predicted != targetLabel => "False Negative (FN)",
        var (actual, predicted) when actual != targetLabel && predicted != targetLabel => "True Negative (TN)",
        _ => "Unknown"
    };

// Using record classes with init properties for deserialization
public record TrainingData
{
    [JsonPropertyName("ItemDescription")]
    public string ItemDescription { get; init; }

    [JsonPropertyName("ActualLabel")]
    public string ActualLabel { get; init; }

    [JsonPropertyName("PredictedLabel")]
    public string PredictedLabel { get; init; }
}

public record TrainingDataContainer
{
    [JsonPropertyName("data")]
    public List<TrainingData> Data { get; init; }
}

// Prediction class, using records with calculated properties
public record Prediction(List<TrainingData> Data, string TargetLabel)
{
    public int TruePositives => Data.Count(kv => kv.ActualLabel == TargetLabel && kv.PredictedLabel == TargetLabel);
    public int FalsePositives => Data.Count(kv => kv.ActualLabel != TargetLabel && kv.PredictedLabel == TargetLabel);
    public int FalseNegatives => Data.Count(kv => kv.ActualLabel == TargetLabel && kv.PredictedLabel != TargetLabel);
    public int TrueNegatives => Data.Count(kv => kv.ActualLabel != TargetLabel && kv.PredictedLabel != TargetLabel);
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
        // Precision: TP / (TP + FP)
        Precision = (double)_prediction.TruePositives / (_prediction.TruePositives + _prediction.FalsePositives);

        // Recall: TP / (TP + FN)
        Recall = (double)_prediction.TruePositives / (_prediction.TruePositives + _prediction.FalseNegatives);

        // F1 Score: 2 * (Precision * Recall) / (Precision + Recall)
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
