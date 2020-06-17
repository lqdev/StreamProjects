// Learn more about F# at http://fsharp.org

open System
open System.IO
open Microsoft.ML

[<CLIMutable>]
type ImageData = {
    Id: string
    ImagePath:string
    Label:string
}

[<CLIMutable>]
type ImagePrediction = {
    Id: string
    PredictedLabel:string
    Score: single array
}

[<EntryPoint>]
let main argv =

    // Initialize ML Context
    let ctx = MLContext()

    // Loaded the trained model
    let mutable dvSchema = Unchecked.defaultof<DataViewSchema>  
    let model = ctx.Model.Load("MLModel.zip", &dvSchema)
   
    // Use the model to make predictions
    let testPredictions = 
        Directory.GetFiles("/datadrive/testImages")
        |> Array.map(fun filePath -> 
            let imageName = Path.GetFileNameWithoutExtension(filePath)
            {Id=imageName;ImagePath=filePath;Label=""})
        |> ctx.Data.LoadFromEnumerable
        |> model.Transform

    // Inspect predictions
    let predictionEnumerable =
        ctx.Data.CreateEnumerable<ImagePrediction>(testPredictions,true)

    let header = [
        "Id"
        "Prediction1"
        "Prediction2"
        "Prediction3"
        "Prediction4"
        "Prediction5"
        "Prediction6"
        "Prediction7"
        "Prediction8"
        "Prediction9"
    ]

    let predictionProbabilities = 
        predictionEnumerable
        |> Seq.map(fun pred -> sprintf "%s,%s" pred.Id (String.Join(',',pred.Score)))
        |> List.ofSeq

    let data = [String.Join(',',header)] @ predictionProbabilities

    File.WriteAllLines("submission.csv",data)

    0 // return an integer exit code
