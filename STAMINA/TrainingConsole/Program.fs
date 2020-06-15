// Learn more about F# at http://fsharp.org

open System
open System.IO
open Microsoft.ML
open Microsoft.ML.Vision
open Microsoft.ML.Data

[<CLIMutable>]
type ImageData = {
    ImagePath:string
    Label:string
}

let getImageData trainFileName (sep:char) hasHeader dataDir ext = 
    
    let skipRows = 
        match hasHeader with
        | true -> 1
        | false -> 0
    
    File.ReadAllLines trainFileName
    |> Array.skip skipRows
    |> Array.map(fun row -> 
        let (cols:string array) = row.Split(sep)

        let trimmedFileName = cols.[0].Trim('"')

        let imagePath = sprintf "%s/%s.%s" dataDir trimmedFileName ext
        {
            ImagePath=imagePath
            Label=cols.[1]
        }
    )

[<EntryPoint>]
let main argv =
    
    let ctx = MLContext()

    let imageData = getImageData "/datadrive/trainLabels.csv" ',' true "/datadrive/trainImages" "jpg"

    // Load data into IDataView
    let imageIdv = ctx.Data.LoadFromEnumerable imageData

    // Create preprocessing pipeline
    let preprocessingPipeline = 
        EstimatorChain()
            .Append(ctx.Transforms.LoadRawImageBytes("Image",null,"ImagePath"))
            .Append(ctx.Transforms.Conversion.MapValueToKey("LabelAsKey","Label"))
  
    // Create training pipeline 
    let trainerOptions = ImageClassificationTrainer.Options()
    trainerOptions.Arch <- ImageClassificationTrainer.Architecture.InceptionV3
    trainerOptions.FeatureColumnName <- "Image" 
    trainerOptions.LabelColumnName <- "LabelAsKey"
    trainerOptions.WorkspacePath <- "workspace"
    trainerOptions.MetricsCallback <- Action<ImageClassificationTrainer.ImageClassificationMetrics>(fun x -> printfn "%s" (x.ToString()))

    let trainerPipeline = EstimatorChain().Append(ctx.MulticlassClassification.Trainers.ImageClassification(trainerOptions))

    let postProcessingPipeline = EstimatorChain().Append(ctx.Transforms.Conversion.MapKeyToValue("PredictedLabel"))

    let pipeline = preprocessingPipeline.Append(trainerPipeline).Append(postProcessingPipeline)

    let model = 
        imageIdv |> pipeline.Fit

    ctx.Model.Save(model,imageIdv.Schema,"MLModel.zip")

    0 // return an integer exit code
