(*Install NuGet packages*)
#r "nuget:Microsoft.ML,1.5.2"
#r "nuget:Microsoft.ML.AutoML,0.17.2"

(*Import packages*)
open System
open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.AutoML

(*Define paths*)
// let trainDataPath = "/datadrive/Data/ArXivTrainData/*"
let trainDataPath = "/datadrive/Data/ArXivTrainData/part-00000-f96ec43e-ba4c-462c-8793-cc605d146665-c000.csv"
let labelColumnName = "categories"

(*Initialize MLContext*)
let mlContext = MLContext()

(*Define Data Schema*)
[<CLIMutable>]
type ArxivData = {
    [<LoadColumn(0)>] Title : string
    [<LoadColumn(1)>] Abstract : string
    [<LoadColumn(2)>] Categories : string
}

(*Load data into IDataView*)
let arxivDataView = 
    mlContext
        .Data
        .LoadFromTextFile<ArxivData>(trainDataPath,separatorChar=',',hasHeader=true)

let pipeline = 
    EstimatorChain()
        .Append(mlContext.Transforms.Text.FeaturizeText("TitleFeaturized","Title"))
        .Append(mlContext.Transforms.Text.FeaturizeText("AbstractFeaturized","Abstract"))
        .Append(mlContext.Transforms.Concatenate("Features","TitleFeaturized","AbstractFeaturized"))
        .Append(mlContext.Transforms.Conversion.MapValueToKey("Label","Categories"))
        // .Append(mlContext.Transforms.CopyColumns("Label","Categories"))
        .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())

let model = pipeline.Fit(arxivDataView)

(*Define experiment settigns*)
// let arxivExperimentSettings = new MulticlassExperimentSettings()
// arxivExperimentSettings.MaxExperimentTimeInSeconds <- 60u
// arxivExperimentSettings.OptimizingMetric <- MulticlassClassificationMetric.LogLoss

(*Define progress handler*)

// type MulticlassProgressHandler () = 
//     interface IProgress<RunDetail<MulticlassClassificationMetrics>> with

//         member this.Report(run: RunDetail<MulticlassClassificationMetrics>) = 
//             printfn "Trained %s with Log Loss %f" run.TrainerName run.ValidationMetrics.LogLoss

(*Create experiment*)
// let arxivExperiment = 
//     mlContext
//         .Auto()
//         .CreateMulticlassClassificationExperiment(arxivExperimentSettings)

(*Run experiment*)
// let experimentResults = 
//     arxivExperiment
//         .Execute(arxivDataView,labelColumnName="Categories")
