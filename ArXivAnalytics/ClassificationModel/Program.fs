// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.ML
open Microsoft.ML.Data

(*Define Data Schema*)
[<CLIMutable>]
type ArxivData = {
    [<JsonPropertyName("title")>]
    Title : string
    
    [<JsonPropertyName("abstract")>]
    Abstract : string
    
    [<JsonPropertyName("categories")>]
    Categories : string
}

let loadData (file:String) = seq {
    use reader = new StreamReader(file)
    while not reader.EndOfStream do
        let line = reader.ReadLine()
        yield JsonSerializer.Deserialize<ArxivData>(line)
}

[<EntryPoint>]
let main argv =

    (*Define paths*)
    let trainDataPath = "/datadrive/Data/ArXivTrainData/"
    let labelColumnName = "Categories"

    (*Data*)
    let trainFile = 
        Directory.GetFiles(trainDataPath)
        |> Array.filter(fun file -> Path.GetExtension(file) = ".json")
        |> Seq.head    

    printfn "Train File: %s" trainFile

    let trainData = loadData trainFile |> Seq.take 250000

    (*Initialize MLContext*)
    let mlContext = MLContext()

    (*Load data into IDataView*)
    let arxivDataView = 
        mlContext
            .Data
            .LoadFromEnumerable<ArxivData>(trainData)

    (*Define pipeline*)
    let pipeline = 
        EstimatorChain()
            .Append(mlContext.Transforms.Text.FeaturizeText("TitleFeaturized","Title"))
            .Append(mlContext.Transforms.Text.FeaturizeText("AbstractFeaturized","Abstract"))
            .Append(mlContext.Transforms.Concatenate("Features","TitleFeaturized","AbstractFeaturized"))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label","Categories"))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())

    printfn "Training model"

    (*Train model*)
    let model = pipeline.Fit(arxivDataView)

    printfn "Finished training"

    printfn "Saving model"
    
    mlContext.Model.Save(model,arxivDataView.Schema,"arxiv-model.zip")
    
    printfn "Model saved"

    let predictions = model.Transform(arxivDataView)

    printfn "Evaluating model"

    let eval = 
         mlContext
             .MulticlassClassification
             .Evaluate(predictions)

    printfn "Log Loss: %f" eval.LogLoss

    0 // return an integer exit code
