// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open Microsoft.ML
open Microsoft.ML.Data

(*Define Data Schema*)
[<CLIMutable>]
type ArxivData = {
    [<LoadColumn(0)>] Title : string
    [<LoadColumn(1)>] Abstract : string
    [<LoadColumn(2)>] Categories : string
}

[<EntryPoint>]
let main argv =

    (*Define paths*)
    let trainDataPath = "/datadrive/Data/ArXivTrainData/part-00000-f96ec43e-ba4c-462c-8793-cc605d146665-c000.csv"
    let labelColumnName = "Categories"
    
    (*Initialize MLContext*)
    let mlContext = MLContext()

    (*Load data into IDataView*)
    let textLoaderOptions = TextLoader.Options()
    textLoaderOptions.AllowQuoting <- true
    textLoaderOptions.HasHeader <- true
    textLoaderOptions.Separators <- [|','|]
    textLoaderOptions.ReadMultilines <- true

    let arxivDataView = 
        mlContext
            .Data
            .LoadFromTextFile<ArxivData>(trainDataPath,textLoaderOptions)

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

    let predictions = model.Transform(arxivDataView)

    printfn "Evaluating model"

    let eval = 
        mlContext
            .MulticlassClassification
            .Evaluate(predictions)

    printfn "Log Loss: %f" eval.LogLoss

    0 // return an integer exit code